using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class LogsClient : ILogsClient
    {
        private const string _resourceUri = "logs/{0}";

        private readonly string _baseAddress;

        private readonly IHttpContextHeaders _customHeaders;

        private HttpClient _httpClient;

        public LogsClient(string baseAddress, IHttpContextHeaders customHeaders, HttpClient HttpClient)
        {
            _baseAddress = baseAddress;
            _customHeaders = customHeaders;
            _httpClient = HttpClient;
        }

        public async Task<LogsDto> Get(string projectId)
        {
            if(!IsServiceAlive())
            {
                return new LogsDto();
            }

            RestClient<LogsDto> baseClient = GetRestClient(projectId);
            return await baseClient.Get(projectId);
        }

        public async Task<HttpResponseMessage> Put(string projectId, LogsDto logs)
        {
            AssertIfServiceNotAlive();

            RestClient<LogsDto> baseClient = GetRestClient(projectId);
            return await baseClient.Put(projectId, logs);
        }

        private void AssertIfServiceNotAlive()
        {
            if(!IsServiceAlive()) 
            {
                throw new ServiceNotAvailableException($"Microservice with tag 'depository' is not running!");
            }
        }

        private bool IsServiceAlive()
        {
            return !string.IsNullOrWhiteSpace(_baseAddress);
        }

        private RestClient<LogsDto> GetRestClient(string projectId)
        {            
            var uri = string.Format(_resourceUri, projectId);
            string resourceAddress = Path.Combine(_baseAddress, uri);

            var baseClient = new RestClient<LogsDto>(_httpClient, resourceAddress, _customHeaders);
            return baseClient;
        }
    }
}