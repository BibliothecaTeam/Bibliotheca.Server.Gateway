using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Neutrino.AspNetCore.Client;
using Neutrino.Entities.Model;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class ServicesService : IServicesService
    {
        private const string _tagsCacheKey = "ServicesService";

        private readonly INeutrinoClient _neutrinoClient;

        private readonly ApplicationParameters _applicationParameters;

        public ServicesService(
            INeutrinoClient neutrinoClient, 
            IOptions<ApplicationParameters> applicationParameters)
        {
            _neutrinoClient = neutrinoClient;
            _applicationParameters = applicationParameters.Value;
        }

        public async Task<IList<Service>> GetServicesAsync()
        {
            var services = await _neutrinoClient.GetServicesAsync();
            return services;
        }

        public async Task<Service> GetServiceInstanceAsync(string serviceType) 
        {
            var services = await _neutrinoClient.GetServicesByServiceTypeAsync(serviceType);
            return services.FirstOrDefault();
        }
    }
}