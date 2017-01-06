namespace Bibliotheca.Server.Gateway.Core.Parameters
{
    public class ApplicationParameters
    {
        public string OAuthAuthority { get; set; }

        public string OAuthAudience { get; set; }

        public string SecureToken { get; set; }

        public ServiceDiscovery ServiceDiscovery { get; set; }
    }
}
