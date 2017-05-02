using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.Gateway.Core.Services;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient.Model;
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
        [ProducesResponseType(200, Type = typeof(IList<ServiceDto>))]
        public async Task<IList<ServiceDto>> Get()
        {
            var servicesDtos = await _servicesService.GetServicesAsync();
            return servicesDtos;
        }
    }
}