using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.Gateway.Core.Services;
using Bibliotheca.Server.Mvc.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Neutrino.Entities.Model;

namespace Bibliotheca.Server.Gateway.Api
{
    /// <summary>
    /// Services controller.
    /// </summary>
    [UserAuthorize]
    [ApiVersion("1.0")]
    [Route("api/services")]
    public class ServicesController : Controller
    {
        private readonly IServicesService _servicesService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="servicesService">Service for services information.</param>
        public ServicesController(IServicesService servicesService)
        {
            _servicesService = servicesService;
        }

        /// <summary>
        /// Get information about all running services.
        /// </summary>
        /// <remarks>
        /// Endpoint returns list of running services.
        /// </remarks>
        /// <returns>Information abouth services.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<Service>))]
        public async Task<IList<Service>> Get()
        {
            var services = await _servicesService.GetServicesAsync();
            return services;
        }

        /// <summary>
        /// Get information about service health.
        /// </summary>
        /// <remarks>
        /// Endpoint returns information about service health.
        /// </remarks>
        /// <param name="serviceId">Service id.</param>
        /// <returns>Information abouth service health.</returns>
        [HttpGet("{serviceId}/current-health")]
        [ProducesResponseType(200, Type = typeof(ServiceHealth))]
        public async Task<ServiceHealth> GetHealth(string serviceId)
        {
            var serviceHealth = await _servicesService.GetServiceHealthAsync(serviceId);
            return serviceHealth;
        }
    }
}