using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class SearchClient : BaseHttpClient, ISearchClient
    {
        private readonly string _baseAddress;

        private readonly ILogger _logger;

        public SearchClient(string baseAddress, IDictionary<string, StringValues> customHeaders, ILogger<SearchClient> logger)
         : base(customHeaders)
        {
            _baseAddress = baseAddress;
            _logger = logger;
        }

        public async Task<DocumentSearchResultDto<DocumentIndexDto>> Get(FilterDto filter)
        {
            HttpClient client = GetClient();
            var query = CreateQuery(filter);
            var requestUri = Path.Combine(_baseAddress, $"search?{query}");

            _logger.LogInformation($"Indexer client request (GET): {requestUri}");
            var responseString = await client.GetStringAsync(requestUri);

            var deserializedObject = JsonConvert.DeserializeObject<DocumentSearchResultDto<DocumentIndexDto>>(responseString);

            return deserializedObject;
        }

        public async Task<DocumentSearchResultDto<DocumentIndexDto>> Get(FilterDto filter, string projectId, string branchName)
        {
            HttpClient client = GetClient();
            var query = CreateQuery(filter);
            var requestUri = Path.Combine(_baseAddress, $"search/projects/{projectId}/branches/{branchName}?{query}");

            _logger.LogInformation($"Indexer client request (GET): {requestUri}");
            var responseString = await client.GetStringAsync(requestUri);

            var deserializedObject = JsonConvert.DeserializeObject<DocumentSearchResultDto<DocumentIndexDto>>(responseString);

            return deserializedObject;
        }

        public async Task<HttpResponseMessage> Post(string projectId, string branchName, IEnumerable<DocumentIndexDto> documentIndexDtos)
        {
            HttpClient client = GetClient();
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
            HttpClient client = GetClient();
            var requestUri = Path.Combine(_baseAddress, $"search/projects/{projectId}/branches/{branchName}");

            _logger.LogInformation($"Indexer client request (DELETE): {requestUri}");
            var httpResponseMessage = await client.DeleteAsync(requestUri);

            return httpResponseMessage;
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
