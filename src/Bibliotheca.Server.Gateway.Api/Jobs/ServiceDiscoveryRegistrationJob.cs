using System;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient;
using Hangfire.Server;
using Microsoft.Extensions.Options;

namespace Bibliotheca.Server.Gateway.Api.Jobs
{
    /// <summary>
    /// Job for register in service discovery.
    /// </summary>
    public class ServiceDiscoveryRegistrationJob : IServiceDiscoveryRegistrationJob
    {
        private readonly IServiceDiscoveryClient _serviceDiscoveryClient;

        private readonly ApplicationParameters _applicationParameters;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceDiscoveryClient">Service discovery client.</param>
        /// <param name="options">Application parameters.</param>
        public ServiceDiscoveryRegistrationJob(IServiceDiscoveryClient serviceDiscoveryClient, IOptions<ApplicationParameters> options)
        {
            _serviceDiscoveryClient = serviceDiscoveryClient;
            _applicationParameters = options.Value;
        }

        /// <summary>
        /// Register service in service discovery application.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <returns>Returns async task.</returns>
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