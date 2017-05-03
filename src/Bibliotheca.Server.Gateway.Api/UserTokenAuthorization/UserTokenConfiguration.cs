using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.Mvc.Middleware.Authorization.UserTokenAuthentication;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bibliotheca.Server.Gateway.Api.UserTokenAuthorization
{
    /// <summary>
    /// Class which is used to retrieve address to authorization service.
    /// </summary>
    public class UserTokenConfiguration : IUserTokenConfiguration
    {
        private readonly ILogger<UserTokenConfiguration> _logger;

        private readonly IServiceDiscoveryQuery _serviceDiscoveryQuery;

        IOptions<ApplicationParameters> _applicationParameters;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="serviceDiscoveryQuery">Service discovery query.</param>
        /// <param name="applicationParameters">Application parameters.</param>
        public UserTokenConfiguration(
            ILogger<UserTokenConfiguration> logger, 
            IServiceDiscoveryQuery serviceDiscoveryQuery, 
            IOptions<ApplicationParameters> applicationParameters)
        {
            _logger = logger;
            _serviceDiscoveryQuery = serviceDiscoveryQuery;
            _applicationParameters = applicationParameters;
        }

        /// <summary>
        /// Get url to authorization service.
        /// </summary>
        /// <returns>Url to authorization service.</returns>
        public string GetAuthorizationUrl()
        {
            _logger.LogInformation("Retrieving authorization url...");

            var instance = _serviceDiscoveryQuery.GetServiceInstanceAsync(
                new ServerOptions { Address = _applicationParameters.Value.ServiceDiscovery.ServerAddress },
                new string[] { "authorization" }
            ).GetAwaiter().GetResult();

            if (instance != null)
            {
                var address = $"http://{instance.Address}:{instance.Port}/api/";
                _logger.LogInformation($"Authorization url was retrieved ({address}).");

                return address;
            }

            _logger.LogInformation($"Authorization url was not retrieved.");
            return null;
        }
    }
}