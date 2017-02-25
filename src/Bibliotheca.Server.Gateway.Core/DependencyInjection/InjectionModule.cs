using Autofac;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Bibliotheca.Server.Depository.Client;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Microsoft.Extensions.Options;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Autofac.Core;
using Bibliotheca.Server.Indexer.Client;

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
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            var serviceAssembly = typeof(InjectionModule).GetTypeInfo().Assembly;

            builder.RegisterAssemblyTypes(serviceAssembly)
                .Where(t => t.Name.EndsWith("Service"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
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
            var serviceDiscoveryQuery = c.Resolve<IServiceDiscoveryQuery>();
            var applicationParameters = c.Resolve<IOptions<ApplicationParameters>>();

            var service = serviceDiscoveryQuery.GetServiceAsync(
                new ServerOptions { Address = applicationParameters.Value.ServiceDiscovery.ServerAddress },
                new string[] { serviceTag }
            ).GetAwaiter().GetResult();

            if (service == null)
            {
                throw new DepositoryServiceNotAvailableException($"Microservice with tag '{serviceTag}' service is not running!");
            }

            var address = $"http://{service.Address}:{service.Port}/api/";
            return address;
        }
    }
}