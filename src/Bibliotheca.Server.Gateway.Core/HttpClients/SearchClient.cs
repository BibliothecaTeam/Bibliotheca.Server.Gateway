using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class SearchClient : ISearchClient
    {
        private readonly string _baseAddress;

        private readonly IDictionary<string, StringValues> _customHeaders;

        private readonly ILogger _logger;

        private readonly HttpClient _httpClient;

        public SearchClient(
            string baseAddress, 
            IDictionary<string, StringValues> customHeaders, 
            ILogger<SearchClient> logger,
            HttpClient httpClient)
        {
            _baseAddress = baseAddress;
            _customHeaders = customHeaders;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task<DocumentSearchResultDto<DocumentIndexDto>> Get(FilterDto filter)
        {
            if(!IsServiceAlive())
            {
                return new DocumentSearchResultDto<DocumentIndexDto>();
            }

            var client = GetClient();
            var query = CreateQuery(filter);

            var requestUri = Path.Combine(_baseAddress, $"search");
            if(!string.IsNullOrWhiteSpace(query))
            {
                requestUri += $"?{query}";
            }

            _logger.LogInformation($"Indexer client request (GET): {requestUri}");
            var response = await client.GetAsync(requestUri);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var deserializedObject = JsonConvert.DeserializeObject<DocumentSearchResultDto<DocumentIndexDto>>(responseString);
                return deserializedObject;
            }

            return new DocumentSearchResultDto<DocumentIndexDto>();
        }

        public async Task<DocumentSearchResultDto<DocumentIndexDto>> Get(FilterDto filter, string projectId, string branchName)
        {
            if(!IsServiceAlive())
            {
                return new DocumentSearchResultDto<DocumentIndexDto>();
            }

            var client = GetClient();
            var query = CreateQuery(filter);

            var requestUri = Path.Combine(_baseAddress, $"search/projects/{projectId}/branches/{branchName}");
            if(!string.IsNullOrWhiteSpace(query))
            {
                requestUri += $"?{query}";
            }

            _logger.LogInformation($"Indexer client request (GET): {requestUri}");
            var response = await client.GetAsync(requestUri);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var deserializedObject = JsonConvert.DeserializeObject<DocumentSearchResultDto<DocumentIndexDto>>(responseString);
                return deserializedObject;
            }

            return new DocumentSearchResultDto<DocumentIndexDto>();
        }

        public async Task<HttpResponseMessage> Post(string projectId, string branchName, IEnumerable<DocumentIndexDto> documentIndexDtos)
        {
            AssertIfServiceNotAlive();

            var client = GetClient();
            var requestUri = Path.Combine(_baseAddress, $"search/projects/{projectId}/branches/{branchName}");

            var serializedObject = JsonConvert.SerializeObject(documentIndexDtos);
            HttpContent content = new StringContent(serializedObject);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            _logger.LogInformation($"Indexer client request (POST): {requestUri}");
            var httpResponseMessage = await client.PostAsync(requestUri, content);

            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> Delete(string projectId, string branchName)
        {
            AssertIfServiceNotAlive();

            var client = GetClient();
            var requestUri = Path.Combine(_baseAddress, $"search/projects/{projectId}/branches/{branchName}");

            _logger.LogInformation($"Indexer client request (DELETE): {requestUri}");
            var httpResponseMessage = await client.DeleteAsync(requestUri);

            return httpResponseMessage;
        }

        private BaseHttpClient GetClient()
        {            
            var baseClient = new BaseHttpClient(_httpClient, _customHeaders);
            return baseClient;
        }

        public bool IsServiceAlive()
        {
            return !string.IsNullOrWhiteSpace(_baseAddress);
        }

        private void AssertIfServiceNotAlive()
        {
            if(!IsServiceAlive()) 
            {
                throw new ServiceNotAvailableException($"Microservice with tag 'authorization' is not running!");
            }
        }
        private string CreateQuery(FilterDto filter)
        {
            string query = $"limit={filter.Limit}";
            query += $"&page={filter.Page}";

            if (!string.IsNullOrWhiteSpace(filter.Query))
            {
                query += $"&query={filter.Query}";
            }

            return query;
        }
    }
}
