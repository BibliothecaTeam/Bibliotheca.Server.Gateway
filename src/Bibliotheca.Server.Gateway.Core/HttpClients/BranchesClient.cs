using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
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
            if(!IsServiceAlive())
            {
                return new List<BranchDto>();
            }

            RestClient<BranchDto> baseClient = GetRestClient(projectId);
            return await baseClient.Get();
        }

        public async Task<BranchDto> Get(string projectId, string branchName)
        {
            if(!IsServiceAlive())
            {
                return null;
            }

            RestClient<BranchDto> baseClient = GetRestClient(projectId);
            return await baseClient.Get(branchName);
        }

        public async Task<HttpResponseMessage> Post(string projectId, BranchDto branch)
        {
            AssertIfServiceNotAlive();

            RestClient<BranchDto> baseClient = GetRestClient(projectId);
            return await baseClient.Post(branch);
        }

        public async Task<HttpResponseMessage> Put(string projectId, string branchName, BranchDto branch)
        {
            AssertIfServiceNotAlive();

            RestClient<BranchDto> baseClient = GetRestClient(projectId);
            return await baseClient.Put(branchName, branch);
        }

        public async Task<HttpResponseMessage> Delete(string projectId, string branchName)
        {
            AssertIfServiceNotAlive();

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