using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class ProjectsClient : IProjectsClient
    {
        private const string _resourceUri = "projects";

        private readonly string _baseAddress;

        private readonly IDictionary<string, StringValues> _customHeaders;

        private readonly HttpClient _httpClient;

        public IDictionary<string, StringValues> CustomHeaders
        {
            get { return _customHeaders; }
        }

        public ProjectsClient(string baseAddress, IDictionary<string, StringValues> customHeaders, HttpClient httpClient)
        {
            _baseAddress = baseAddress;
            _customHeaders = customHeaders;
            _httpClient = httpClient;
        }

        public async Task<IList<ProjectDto>> Get()
        {
            if(!IsServiceAlive())
            {
                return new List<ProjectDto>();
            }

            RestClient<ProjectDto> baseClient = GetRestClient();
            return await baseClient.Get();
        }

        public async Task<ProjectDto> Get(string projectId)
        {
            if(!IsServiceAlive())
            {
                return null;
            }

            RestClient<ProjectDto> baseClient = GetRestClient();
            return await baseClient.Get(projectId);
        }

        public async Task<HttpResponseMessage> Post(ProjectDto project)
        {
            AssertIfServiceNotAlive();

            RestClient<ProjectDto> baseClient = GetRestClient();
            return await baseClient.Post(project);
        }

        public async Task<HttpResponseMessage> Put(string projectId, ProjectDto project)
        {
            AssertIfServiceNotAlive();

            RestClient<ProjectDto> baseClient = GetRestClient();
            return await baseClient.Put(projectId, project);
        }

        public async Task<HttpResponseMessage> Delete(string projectId)
        {
            AssertIfServiceNotAlive();

            RestClient<ProjectDto> baseClient = GetRestClient();
            return await baseClient.Delete(projectId);
        }

        private RestClient<ProjectDto> GetRestClient()
        {
            string resourceAddress = Path.Combine(_baseAddress, _resourceUri);
            var baseClient = new RestClient<ProjectDto>(_httpClient, resourceAddress, _customHeaders);
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