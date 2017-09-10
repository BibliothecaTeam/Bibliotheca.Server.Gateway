using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Neutrino.Entities.Model;

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
        [ProducesResponseType(200, Type = typeof(IList<Service>))]
        public async Task<IList<Service>> Get()
        {
            var services = await _servicesService.GetServicesAsync();
            return services;
        }
    }
}