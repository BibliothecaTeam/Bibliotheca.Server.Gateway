using System;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient;
using Hangfire.Server;
using Microsoft.Extensions.Options;

namespace Bibliotheca.Server.Gateway.Api.Jobs
{
    public class ServiceDiscoveryRegistrationJob : IServiceDiscoveryRegistrationJob
    {
        private readonly IServiceDiscoveryClient _serviceDiscoveryClient;

        private readonly ApplicationParameters _applicationParameters;

        public ServiceDiscoveryRegistrationJob(IServiceDiscoveryClient serviceDiscoveryClient, IOptions<ApplicationParameters> options)
        {
            _serviceDiscoveryClient = serviceDiscoveryClient;
            _applicationParameters = options.Value;
        }

        public async Task RegisterServiceAsync(PerformContext context)
        {
            var serviceDiscoveryOptions = GetServiceDiscoveryOptions();
            await _serviceDiscoveryClient.RegisterAsync(serviceDiscoveryOptions);
        }

        private ServiceDiscoveryOptions GetServiceDiscoveryOptions()
        {
            var options = new ServiceDiscoveryOptions();
            options.ServiceOptions.Id = _applicationParameters.ServiceDiscovery.ServiceId;
            options.ServiceOptions.Name = _applicationParameters.ServiceDiscovery.ServiceName;
            options.ServiceOptions.Address = _applicationParameters.ServiceDiscovery.ServiceAddress;
            options.ServiceOptions.Port = Convert.ToInt32(_applicationParameters.ServiceDiscovery.ServicePort);
            options.ServiceOptions.HttpHealthCheck = _applicationParameters.ServiceDiscovery.ServiceHttpHealthCheck;
            options.ServiceOptions.Tags = _applicationParameters.ServiceDiscovery.ServiceTags;
            options.ServerOptions.Address = _applicationParameters.ServiceDiscovery.ServerAddress;

            return options;
        }
    }
}