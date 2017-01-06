namespace Bibliotheca.Server.Gateway.Core.Parameters
{
    public class ServiceDiscovery
    {
        public string ServiceId { get; set; }

        public string ServiceName { get; set; }

        public string ServiceAddress { get; set; }

        public string ServicePort { get; set; }

        public string ServiceHttpHealthCheck { get; set; }

        public string[] ServiceTags { get; set; }
        
        public string ServerAddress { get; set; }
    }
}