namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class InstanceDto
    {
        public string Id { get; set; }

        public string Address { get; set; }

        public int Port { get; set; }

        public string HealthStatus { get; set; }

        public string HealthOuptput { get; set; }
    }
}