using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class NightcrawlerClient : BaseHttpClient, INightcrawlerClient
    {
        private readonly string _baseAddress;

        private readonly IDictionary<string, StringValues> _customHeaders;

        private readonly ILogger _logger;

        public NightcrawlerClient(string baseAddress, IDictionary<string, StringValues> customHeaders, ILogger<NightcrawlerClient> logger) 
            : base(customHeaders)
        {
            _baseAddress = baseAddress;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> Post(string projectId, string branchName)
        {
            HttpClient client = GetClient();
            var requestUri = Path.Combine(_baseAddress, $"queues/{projectId}/{branchName}");

            _logger.LogInformation($"Nightcrawler client request (POST): {requestUri}");
            var httpResponseMessage = await client.PostAsync(requestUri, null);

            return httpResponseMessage;
        }

        public async Task<IndexStatusDto> Get(string projectId, string branchName)
        {
            HttpClient client = GetClient();
            var requestUri = Path.Combine(_baseAddress, $"queues/{projectId}/{branchName}");

            _logger.LogInformation($"Nightcrawler client request (GET): {requestUri}");
            var responseString = await client.GetStringAsync(requestUri);

            var deserializedObject = JsonConvert.DeserializeObject<IndexStatusDto>(responseString);
            return deserializedObject;
        }
    }
}