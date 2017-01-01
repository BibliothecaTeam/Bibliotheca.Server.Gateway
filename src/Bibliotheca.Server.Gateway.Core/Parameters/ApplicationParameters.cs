namespace Bibliotheca.Server.Gateway.Core.Parameters
{
    public class ApplicationParameters
    {
        public string ProjectsUrl { get; set; }
        public string AzureStorageConnectionString { get;set; }
        public string AzureSearchServiceName { get; set; }
        public string AzureSearchApiKey { get; set; }
        public string SecurityToken { get; set; }
        public string CustomStyles { get; set; }
        public string Favicon { get; set; }
        public string Title { get; set; }
        public string CustomLogo { get; set; }
        public bool EnableApplicationInsights { get; set; }
    }
}
