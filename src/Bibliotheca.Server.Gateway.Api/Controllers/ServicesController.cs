using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Bibliotheca.Server.Gateway.Api
{
    /// <summary>
    /// Services controller.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/services")]
    public class ServicesController : Controller
    {
        private readonly IServiceDiscoveryQuery _serviceDiscoveryQuery;

        private readonly ApplicationParameters _applicationParameters;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serviceDiscoveryQuery">Service discovery query service.</param>
        /// <param name="applicationParameters">Application parameters.</param>
        public ServicesController(IServiceDiscoveryQuery serviceDiscoveryQuery, IOptions<ApplicationParameters> applicationParameters)
        {
            _serviceDiscoveryQuery = serviceDiscoveryQuery;
            _applicationParameters = applicationParameters.Value;
        }

        /// <summary>
        /// Get information about all running services.
        /// </summary>
        /// <remarks>
        /// Endpoint returns list of running services.
        /// </remarks>
        /// <returns>Information abouth services.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<ServiceDto>))]
        public async Task<IList<ServiceDto>> Get()
        {
            var serverOptions = new ServerOptions { Address = _applicationParameters.ServiceDiscovery.ServerAddress };
            var services = await _serviceDiscoveryQuery.GetServicesAsync(serverOptions);
            var serviceNames = services.Select(x => x.Service).Distinct();

            var servicesDtos = new List<ServiceDto>();
            foreach(var serviceName in serviceNames)
            {
                var serviceDto = new ServiceDto
                {
                    Name = serviceName
                };

                var servicesHealth = await _serviceDiscoveryQuery.GetServicesHealthAsync(serverOptions, serviceName);
                var instances = services.Where(x => x.Service == serviceName);
                
                foreach(var instance in instances)
                {
                    var instanceDto = new InstanceDto
                    {
                        Address = instance.Address,
                        Id = instance.ID,
                        Port = instance.Port
                    };

                    var healthStatus = servicesHealth.FirstOrDefault(x => x.ServiceID == instance.ID);
                    if(healthStatus != null)
                    {
                        instanceDto.HealthStatus = healthStatus.Status;
                        instanceDto.HealthOuptput = healthStatus.Output;
                    }

                    serviceDto.Instances.Add(instanceDto);
                }

                servicesDtos.Add(serviceDto);
            }

            return servicesDtos;
        }
    }
}