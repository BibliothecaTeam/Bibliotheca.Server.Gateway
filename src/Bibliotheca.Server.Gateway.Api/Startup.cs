using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.Gateway.Core.DependencyInjections;
using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient.Extensions;
using Microsoft.AspNetCore.Http;
using Hangfire;
using Hangfire.MemoryStorage;
using Bibliotheca.Server.Gateway.Api.Jobs;
using Bibliotheca.Server.Gateway.Core.Policies;
using Bibliotheca.Server.Mvc.Middleware.Authorization.UserTokenAuthentication;
using Bibliotheca.Server.Gateway.Api.UserTokenAuthorization;
using Bibliotheca.Server.Mvc.Middleware.Authorization.SecureTokenAuthentication;
using Bibliotheca.Server.Mvc.Middleware.Authorization.BearerAuthentication;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Swashbuckle.AspNetCore.Swagger;

namespace Bibliotheca.Server.Gateway.Api
{
    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup
    {
        private IConfigurationRoot Configuration { get; }

        private bool UseServiceDiscovery { get; set; } = true;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="env">Environment parameters.</param>
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        /// <summary>
        /// Service configuration.
        /// </summary>
        /// <param name="services">List of services.</param>
        /// <returns>Service provider.</returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApplicationParameters>(Configuration);

            if (UseServiceDiscovery)
            {
                services.AddHangfire(x => x.UseStorage(new MemoryStorage()));
            }

            services.AddMemoryCache();
            services.AddOptions();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(SecureTokenDefaults.AuthenticationScheme)
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
            });

            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new QueryStringOrHeaderApiVersionReader("api-version");
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Bibliotheca Gateway API",
                    Description = "Bibliotheca Gateway service is responsible for communication between all application microservices. Also it's main API endpoint for Bibliotheca clients such as Bibliotheca.Client (Angular SPA) or custom scripts.",
                    TermsOfService = "None"
                });

                options.AddSecurityDefinition("apiKey", new ApiKeyScheme
                {
                    Name = "Authorization",
                    Type = "apiKey",
                    In = "header",
                    Description = "As a authorization header you can send one of the following token: <br />" +
                    " - Bearer <AccessToken> - JWT token obtained by OAuth2 authorization <br />" +
                    " - SecureToken <GUID> - global token defined as a variable in services parameters <br />" +
                    " - UserToken <GUID> - token generated on user property page <br />" +
                    " - ProjectToken <GUID> - token generated on project property page"
                });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Bibliotheca.Server.Gateway.Api.xml"); 
                options.IncludeXmlComments(xmlPath);
            });

            services.AddServiceDiscovery();

            services.AddScoped<IServiceDiscoveryRegistrationJob, ServiceDiscoveryRegistrationJob>();
            services.AddScoped<IUserTokenConfiguration, UserTokenConfiguration>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanAddProject", policy => policy.Requirements.Add(new HasAccessToCreateProjectRequirement()));
                options.AddPolicy("CanManageUsers", policy => policy.Requirements.Add(new HasAccessToManageUsersRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, HasAccessToCreateProjectHandler>();
            services.AddScoped<IAuthorizationHandler, HasAccessToManageUsersHandler>();
            services.AddScoped<IAuthorizationHandler, ProjectAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, UserAuthorizationHandler>();
            services.AddScoped<IAuthorizationHandler, CanUploadBranchHandler>();

            return services.AddApplicationModules(Configuration);
        }

        /// <summary>
        /// Configure web application.
        /// </summary>
        /// <param name="app">Application builder.</param>
        /// <param name="env">Environment parameters.</param>
        /// <param name="loggerFactory">Logger.</param>
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory
        )
        {
            if(env.IsDevelopment())
            {
                loggerFactory.AddConsole(Configuration.GetSection("Logging"));
                loggerFactory.AddDebug();
            }
            else
            {
                loggerFactory.AddAzureWebAppDiagnostics();
            }

            if (UseServiceDiscovery)
            {
                app.UseHangfireServer();
                RecurringJob.AddOrUpdate<IServiceDiscoveryRegistrationJob>("register-service", x => x.RegisterServiceAsync(null), Cron.Minutely);
            }

            app.UseExceptionHandler();

            app.UseCors("AllowAllOrigins");

            var secureTokenOptions = new SecureTokenOptions
            {
                SecureToken = Configuration["SecureToken"],
                AuthenticationScheme = SecureTokenDefaults.AuthenticationScheme,
                Realm = SecureTokenDefaults.Realm
            };
            app.UseSecureTokenAuthentication(secureTokenOptions);

            var userTokenOptions = new UserTokenOptions
            {
                AuthenticationScheme = UserTokenDefaults.AuthenticationScheme,
                Realm = UserTokenDefaults.Realm
            };
            app.UseUserTokenAuthentication(userTokenOptions);

            var jwtBearerOptions = new JwtBearerOptions
            {
                Authority = Configuration["OAuthAuthority"],
                Audience = Configuration["OAuthAudience"],
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            };
            app.UseBearerAuthentication(jwtBearerOptions);

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
