using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class ProjectsClient : IProjectsClient
    {
        private const string _resourceUri = "projects";

        private readonly string _baseAddress;

        private readonly IDictionary<string, StringValues> _customHeaders;

        public IDictionary<string, StringValues> CustomHeaders
        {
            get { return _customHeaders; }
        }

        public ProjectsClient(string baseAddress, IDictionary<string, StringValues> customHeaders)
        {
            _baseAddress = baseAddress;
            _customHeaders = customHeaders;
        }

        public async Task<IList<ProjectDto>> Get()
        {
            RestClient<ProjectDto> baseClient = GetRestClient();
            return await baseClient.Get();
        }

        public async Task<ProjectDto> Get(string projectId)
        {
            RestClient<ProjectDto> baseClient = GetRestClient();
            return await baseClient.Get(projectId);
        }

        public async Task<HttpResponseMessage> Post(ProjectDto project)
        {
            RestClient<ProjectDto> baseClient = GetRestClient();
            return await baseClient.Post(project);
        }

        public async Task<HttpResponseMessage> Put(string projectId, ProjectDto project)
        {
            RestClient<ProjectDto> baseClient = GetRestClient();
            return await baseClient.Put(projectId, project);
        }

        public async Task<HttpResponseMessage> Delete(string projectId)
        {
            RestClient<ProjectDto> baseClient = GetRestClient();
            return await baseClient.Delete(projectId);
        }

        private RestClient<ProjectDto> GetRestClient()
        {
            string resourceAddress = Path.Combine(_baseAddress, _resourceUri);
            var baseClient = new RestClient<ProjectDto>(resourceAddress, _customHeaders);
            return baseClient;
        }
    }
}