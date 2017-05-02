using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class NightcrawlerClient : BaseHttpClient, INightcrawlerClient
    {
        private readonly string _baseAddress;

        private readonly ILogger _logger;

        public NightcrawlerClient(string baseAddress, IDictionary<string, StringValues> customHeaders, ILogger<NightcrawlerClient> logger) 
            : base(customHeaders)
        {
            _baseAddress = baseAddress;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> Post(string projectId, string branchName)
        {
            AssertIfServiceNotAlive();

            HttpClient client = GetClient();
            var requestUri = Path.Combine(_baseAddress, $"queues/{projectId}/{branchName}");

            _logger.LogInformation($"Nightcrawler client request (POST): {requestUri}");
            var httpResponseMessage = await client.PostAsync(requestUri, null);

            return httpResponseMessage;
        }

        public async Task<IndexStatusDto> Get(string projectId, string branchName)
        {
            if(!IsServiceAlive())
            {
                return new IndexStatusDto();
            }

            HttpClient client = GetClient();
            var requestUri = Path.Combine(_baseAddress, $"queues/{projectId}/{branchName}");

            _logger.LogInformation($"Nightcrawler client request (GET): {requestUri}");
            var responseString = await client.GetStringAsync(requestUri);

            var deserializedObject = JsonConvert.DeserializeObject<IndexStatusDto>(responseString);
            return deserializedObject;
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