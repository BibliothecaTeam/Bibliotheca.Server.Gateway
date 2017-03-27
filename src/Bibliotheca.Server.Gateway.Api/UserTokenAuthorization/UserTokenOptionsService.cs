using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.Mvc.Middleware.Authorization.UserTokenAuthentication;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bibliotheca.Server.Gateway.Api.UserTokenAuthorization
{
    public class UserTokenConfiguration : IUserTokenConfiguration
    {
        private readonly ILogger<UserTokenConfiguration> _logger;

        private readonly IServiceDiscoveryQuery _serviceDiscoveryQuery;

        IOptions<ApplicationParameters> _applicationParameters;
        
        public UserTokenConfiguration(ILogger<UserTokenConfiguration> logger, IServiceDiscoveryQuery serviceDiscoveryQuery, IOptions<ApplicationParameters> applicationParameters)
        {
            _logger = logger;
            _serviceDiscoveryQuery = serviceDiscoveryQuery;
            _applicationParameters = applicationParameters;
        }

        public string GetAuthorizationUrl()
        {
            _logger.LogInformation("Retrieving authorization url...");

            var service = _serviceDiscoveryQuery.GetServiceAsync(
                new ServerOptions { Address = _applicationParameters.Value.ServiceDiscovery.ServerAddress },
                new string[] { "heimdall" }
            ).GetAwaiter().GetResult();

            if (service != null)
            {
                var address = $"http://{service.Address}:{service.Port}/api/";
                _logger.LogInformation($"Authorization url was retrieved ({address}).");

                return address;
            }

            _logger.LogInformation($"Authorization url was not retrieved.");
            return null;
        }
    }
}