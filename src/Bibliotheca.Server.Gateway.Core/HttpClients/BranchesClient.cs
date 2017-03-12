using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class BranchesClient : IBranchesClient
    {
        private const string _resourceUri = "projects/{0}/branches";

        private readonly string _baseAddress;

        private readonly IDictionary<string, StringValues> _customHeaders;

        public BranchesClient(string baseAddress, IDictionary<string, StringValues> customHeaders)
        {
            _baseAddress = baseAddress;
            _customHeaders = customHeaders;
        }

        public async Task<IList<BranchDto>> Get(string projectId)
        {
            RestClient<BranchDto> baseClient = GetRestClient(projectId);
            return await baseClient.Get();
        }

        public async Task<BranchDto> Get(string projectId, string branchName)
        {
            RestClient<BranchDto> baseClient = GetRestClient(projectId);
            return await baseClient.Get(branchName);
        }

        public async Task<HttpResponseMessage> Post(string projectId, BranchDto branch)
        {
            RestClient<BranchDto> baseClient = GetRestClient(projectId);
            return await baseClient.Post(branch);
        }

        public async Task<HttpResponseMessage> Put(string projectId, string branchName, BranchDto branch)
        {
            RestClient<BranchDto> baseClient = GetRestClient(projectId);
            return await baseClient.Put(branchName, branch);
        }

        public async Task<HttpResponseMessage> Delete(string projectId, string branchName)
        {
            RestClient<BranchDto> baseClient = GetRestClient(projectId);
            return await baseClient.Delete(branchName);
        }

        private RestClient<BranchDto> GetRestClient(string projectId)
        {
            var uri = string.Format(_resourceUri, projectId);
            string resourceAddress = Path.Combine(_baseAddress, uri);

            var baseClient = new RestClient<BranchDto>(resourceAddress, _customHeaders);
            return baseClient;
        }
    }
}