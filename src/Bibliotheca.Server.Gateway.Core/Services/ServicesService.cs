using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class ServicesService : IServicesService
    {
        private const string _tagsCacheKey = "ServicesService";

        private readonly IServiceDiscoveryQuery _serviceDiscoveryQuery;

        private readonly ApplicationParameters _applicationParameters;

        public ServicesService(
            IServiceDiscoveryQuery serviceDiscoveryQuery, 
            IOptions<ApplicationParameters> applicationParameters)
        {
            _serviceDiscoveryQuery = serviceDiscoveryQuery;
            _applicationParameters = applicationParameters.Value;
        }

        public async Task<IList<ServiceDto>> GetServicesAsync()
        {
            var serverOptions = new ServerOptions { Address = _applicationParameters.ServiceDiscovery.ServerAddress };
            var services = await _serviceDiscoveryQuery.GetServicesAsync(serverOptions);

            return services;
        }

        public async Task<InstanceDto> GetServiceInstanceAsync(string tag) 
        {
            var serverOptions = new ServerOptions { Address = _applicationParameters.ServiceDiscovery.ServerAddress };
            var instance = await _serviceDiscoveryQuery.GetServiceInstanceAsync(serverOptions, new string[] { tag });

            return instance;
        }
    }
}