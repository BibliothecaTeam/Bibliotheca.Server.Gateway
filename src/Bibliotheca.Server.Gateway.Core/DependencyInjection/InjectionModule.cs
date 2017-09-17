using Autofac;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Microsoft.Extensions.Options;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Autofac.Core;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Bibliotheca.Server.Gateway.Core.Services;
using System;
using Microsoft.Extensions.Logging;
using Flurl;

namespace Bibliotheca.Server.Gateway.Core.DependencyInjection
{
    public class InjectionModule : Autofac.Module
    {
        private readonly IConfigurationRoot _configuration;
        private readonly IServiceCollection _serviceCollection;

        public InjectionModule(IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            _serviceCollection = serviceCollection;
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterServices(builder);
            RegisterDepositoryClients(builder);
            RegisterIndexerClients(builder);
            RegisterCrawlerClients(builder);
            RegisterAuthorizationClients(builder);
            RegisterPdfExportClients(builder);
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            var serviceAssembly = typeof(InjectionModule).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(serviceAssembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

        private void RegisterAuthorizationClients(ContainerBuilder builder)
        {
            var baseAddressParameter = new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "baseAddress",
                    (pi, ctx) => GetServiceAddress(ctx, "authorization"));

            var customHeadersParameter = new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IDictionary<string, StringValues>) && pi.Name == "customHeaders",
                    (pi, ctx) => GetHttpHeaders(ctx));

            builder.RegisterType<UsersClient>().As<IUsersClient>()
                .WithParameter(baseAddressParameter)
                .WithParameter(customHeadersParameter);
        }

        private void RegisterCrawlerClients(ContainerBuilder builder)
        {
            var baseAddressParameter = new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "baseAddress",
                    (pi, ctx) => GetServiceAddress(ctx, "crawler"));

            var customHeadersParameter = new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IDictionary<string, StringValues>) && pi.Name == "customHeaders",
                    (pi, ctx) => GetHttpHeaders(ctx));

            builder.RegisterType<NightcrawlerClient>().As<INightcrawlerClient>()
                .WithParameter(baseAddressParameter)
                .WithParameter(customHeadersParameter);
        }

        private void RegisterDepositoryClients(ContainerBuilder builder)
        {
            var baseAddressParameter = new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "baseAddress",
                    (pi, ctx) => GetServiceAddress(ctx, "depository"));

            var customHeadersParameter = new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IDictionary<string, StringValues>) && pi.Name == "customHeaders",
                    (pi, ctx) => GetHttpHeaders(ctx));

            builder.RegisterType<ProjectsClient>().As<IProjectsClient>()
                .WithParameter(baseAddressParameter)
                .WithParameter(customHeadersParameter);

            builder.RegisterType<BranchesClient>().As<IBranchesClient>()
                .WithParameter(baseAddressParameter)
                .WithParameter(customHeadersParameter);

            builder.RegisterType<DocumentsClient>().As<IDocumentsClient>()
                .WithParameter(baseAddressParameter)
                .WithParameter(customHeadersParameter);

            builder.RegisterType<GroupsClient>().As<IGroupsClient>()
                .WithParameter(baseAddressParameter)
                .WithParameter(customHeadersParameter);
        }

        private void RegisterIndexerClients(ContainerBuilder builder)
        {
            var baseAddressParameter = new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "baseAddress",
                    (pi, ctx) => GetServiceAddress(ctx, "indexer"));

            var customHeadersParameter = new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IDictionary<string, StringValues>) && pi.Name == "customHeaders",
                    (pi, ctx) => GetHttpHeaders(ctx));

            builder.RegisterType<SearchClient>().As<ISearchClient>()
                .WithParameter(baseAddressParameter)
                .WithParameter(customHeadersParameter);
        }

        private void RegisterPdfExportClients(ContainerBuilder builder)
        {
            var baseAddressParameter = new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "baseAddress",
                    (pi, ctx) => GetServiceAddress(ctx, "pdfexport"));

            var customHeadersParameter = new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IDictionary<string, StringValues>) && pi.Name == "customHeaders",
                    (pi, ctx) => GetHttpHeaders(ctx));

            builder.RegisterType<PdfExportClient>().As<IPdfExportClient>()
                .WithParameter(baseAddressParameter)
                .WithParameter(customHeadersParameter);
        }

        private static IDictionary<string, StringValues> GetHttpHeaders(IComponentContext c)
        {
            var httpContextAccessor = c.Resolve<IHttpContextAccessor>();
            var httpContext = httpContextAccessor.HttpContext;

            IDictionary<string, StringValues> headers = null;
            if (httpContext != null && httpContext.Request != null)
            {
                headers = httpContext.Request.Headers as IDictionary<string, StringValues>;
            }

            return headers;
        }

        private static string GetServiceAddress(IComponentContext c, string serviceTag)
        {
            var logger = c.Resolve<ILogger<InjectionModule>>();
            try
            {   
                logger.LogInformation($"Getting address for '{serviceTag}' microservice from cache.");
                var cacheService = c.Resolve<ICacheService>();

                if(!cacheService.TryGetService(serviceTag, out Neutrino.Entities.Model.Service instance))
                {
                    logger.LogInformation($"Getting address for '{serviceTag}' microservice from discovery.");

                    var servicesService = c.Resolve<IServicesService>();
                    instance = servicesService.GetServiceInstanceAsync(serviceTag).GetAwaiter().GetResult();

                    if (instance == null)
                    {
                        logger.LogWarning($"Address for '{serviceTag}' microservice wasn't retrieved from discovery.");
                        return null;
                    }

                    cacheService.AddService(serviceTag, instance);
                }
                
                var address = instance.Address.AppendPathSegment("api/");
                logger.LogInformation($"Address for '{serviceTag}' microservice was retrieved ({address}).");
                return address;
            }
            catch(Exception exception)
            {
                logger.LogError($"Address for '{serviceTag}' microservice wasn't retrieved. There was an exception during retrieving address.");
                logger.LogError($"Exception: {exception}");
                if(exception.InnerException != null)
                {
                    logger.LogError($"Inner exception: {exception.InnerException}");
                }

                return null;
            }
        }
    }
}