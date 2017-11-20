using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class NightcrawlerClient : INightcrawlerClient
    {
        private readonly string _baseAddress;

        private readonly IHttpContextHeaders _customHeaders;

        private readonly ILogger _logger;

        private readonly HttpClient _httpClient;

        public NightcrawlerClient(
            string baseAddress, 
            IHttpContextHeaders customHeaders, 
            ILogger<NightcrawlerClient> logger,
            HttpClient httpClient) 
        {
            _baseAddress = baseAddress;
            _customHeaders = customHeaders;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> Post(string projectId, string branchName)
        {
            AssertIfServiceNotAlive();

            var requestUri = Path.Combine(_baseAddress, $"queues/{projectId}/{branchName}");
            _logger.LogInformation($"Nightcrawler client request (POST): {requestUri}");

            var client = GetClient();
            var httpResponseMessage = await client.PostAsync(requestUri, null);

            return httpResponseMessage;
        }

        public async Task<IndexStatusDto> Get(string projectId, string branchName)
        {
            if(!IsServiceAlive())
            {
                return new IndexStatusDto();
            }

            var requestUri = Path.Combine(_baseAddress, $"queues/{projectId}/{branchName}");
            _logger.LogInformation($"Nightcrawler client request (GET): {requestUri}");

            var client = GetClient();
            var response = await client.GetAsync(requestUri);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var deserializedObject = JsonConvert.DeserializeObject<IndexStatusDto>(responseString);
                return deserializedObject;
            }

            return null;
        }

        private BaseHttpClient GetClient()
        {            
            var baseClient = new BaseHttpClient(_httpClient, _customHeaders);
            return baseClient;
        }

        private void AssertIfServiceNotAlive()
        {
            if(!IsServiceAlive()) 
            {
                throw new ServiceNotAvailableException($"Microservice with tag 'crawler' is not running!");
            }
        }

        private bool IsServiceAlive()
        {
            return !string.IsNullOrWhiteSpace(_baseAddress);
        }
    }
}