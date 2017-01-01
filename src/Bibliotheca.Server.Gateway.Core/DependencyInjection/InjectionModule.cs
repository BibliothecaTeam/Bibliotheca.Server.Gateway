using Autofac;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Bibliotheca.Server.Gateway.Core.Services;

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
            RegisterStorageService(builder);
        }

        private void RegisterServices(ContainerBuilder builder)
        {
            var serviceAssembly = typeof(InjectionModule).GetTypeInfo().Assembly;
            builder.RegisterAssemblyTypes(serviceAssembly)
                .Where(t => t.Name.EndsWith("Service") && !t.Name.EndsWith("StorageService"))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

        private void RegisterStorageService(ContainerBuilder builder)
        {
            if (!string.IsNullOrWhiteSpace(_configuration["AzureStorageConnectionString"]))
            {
                builder.RegisterType<AzureStorageService>().As<IStorageService>().InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<FileStorageService>().As<IStorageService>().InstancePerLifetimeScope();
            }
        }
    }
}