using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class DocumentsClient : IDocumentsClient
    {
        private const string _resourceUri = "projects/{0}/branches/{1}/documents";

        private readonly string _baseAddress;

        private readonly IDictionary<string, StringValues> _customHeaders;

        private readonly HttpClient _httpClient;

        public DocumentsClient(string baseAddress, IDictionary<string, StringValues> customHeaders, HttpClient httpClient)
        {
            _baseAddress = baseAddress;
            _customHeaders = customHeaders;
            _httpClient = httpClient;
        }

        public async Task<IList<BaseDocumentDto>> Get(string projectId, string branchName)
        {
            if(!IsServiceAlive())
            {
                return new List<BaseDocumentDto>();
            }

            RestClient<BaseDocumentDto> baseClient = GetRestClient<BaseDocumentDto>(projectId, branchName);
            return await baseClient.Get();            
        }

        public async Task<DocumentDto> Get(string projectId, string branchName, string fileUri)
        {
            if(!IsServiceAlive())
            {
                return null;
            }

            RestClient<DocumentDto> baseClient = GetRestClient<DocumentDto>(projectId, branchName);
            return await baseClient.Get(fileUri);
        }

        public async Task<HttpResponseMessage> Post(string projectId, string branchName, DocumentDto document)
        {
            AssertIfServiceNotAlive();

            RestClient<DocumentDto> baseClient = GetRestClient<DocumentDto>(projectId, branchName);
            return await baseClient.Post(document);
        }

        public async Task<HttpResponseMessage> Put(string projectId, string branchName, string fileUri, DocumentDto document)
        {
            AssertIfServiceNotAlive();

            RestClient<DocumentDto> baseClient = GetRestClient<DocumentDto>(projectId, branchName);
            return await baseClient.Put(fileUri, document);
        }

        public async Task<HttpResponseMessage> Delete(string projectId, string branchName, string fileUri)
        {
            AssertIfServiceNotAlive();

            RestClient<DocumentDto> baseClient = GetRestClient<DocumentDto>(projectId, branchName);
            return await baseClient.Delete(fileUri);
        }

        private RestClient<T> GetRestClient<T>(string projectId, string branchName) where T: class
        {            
            var uri = string.Format(_resourceUri, projectId, branchName);
            string resourceAddress = Path.Combine(_baseAddress, uri);

            var baseClient = new RestClient<T>(_httpClient, resourceAddress, _customHeaders);
            return baseClient;
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
    }
}