using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Bibliotheca.Server.Gateway.Core.DependencyInjection;

namespace Bibliotheca.Server.Gateway.Core.DependencyInjections
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider AddApplicationModules(this IServiceCollection serviceCollection, IConfigurationRoot configuration)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new InjectionModule(serviceCollection, configuration));
            builder.Populate(serviceCollection);

            var container = builder.Build();
            return container.Resolve<IServiceProvider>();
        }
    }
}
