using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.Mvc.Middleware.Authorization.UserTokenAuthentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Neutrino.AspNetCore.Client;
using Flurl;
using System.Linq;

namespace Bibliotheca.Server.Gateway.Api.UserTokenAuthorization
{
    /// <summary>
    /// Class which is used to retrieve address to authorization service.
    /// </summary>
    public class UserTokenConfiguration : IUserTokenConfiguration
    {
        private readonly ILogger<UserTokenConfiguration> _logger;

        private readonly INeutrinoClient _neutrinoClient;

        IOptions<ApplicationParameters> _applicationParameters;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="neutrinoClient">Service discovery query.</param>
        /// <param name="applicationParameters">Application parameters.</param>
        public UserTokenConfiguration(
            ILogger<UserTokenConfiguration> logger, 
            INeutrinoClient neutrinoClient, 
            IOptions<ApplicationParameters> applicationParameters)
        {
            _logger = logger;
            _neutrinoClient = neutrinoClient;
            _applicationParameters = applicationParameters;
        }

        /// <summary>
        /// Get url to authorization service.
        /// </summary>
        /// <returns>Url to authorization service.</returns>
        public string GetAuthorizationUrl()
        {
            _logger.LogInformation("Retrieving authorization url...");

            var services = _neutrinoClient.GetServicesByServiceTypeAsync("authorization").GetAwaiter().GetResult();
            if (services != null && services.Count > 0)
            {
                var instance = services.FirstOrDefault();
                var address = instance.Address.AppendPathSegment("api/");
                _logger.LogInformation($"Authorization url was retrieved ({address}).");

                return address;
            }

            _logger.LogInformation($"Authorization url was not retrieved.");
            return null;
        }
    }
}