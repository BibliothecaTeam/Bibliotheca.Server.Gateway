using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Microsoft.Extensions.Primitives;
using Flurl;
using Bibliotheca.Server.Gateway.Core.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class GroupsClient : IGroupsClient
    {
        private const string _resourceUri = "groups";

        private readonly string _baseAddress;

        private readonly IDictionary<string, StringValues> _customHeaders;

        private HttpClient _httpClient;

        public GroupsClient(string baseAddress, IDictionary<string, StringValues> customHeaders, HttpClient HttpClient)
        {
            _baseAddress = baseAddress;
            _customHeaders = customHeaders;
            _httpClient = HttpClient;
        }

        public async Task<IList<GroupDto>> Get()
        {
            if(!IsServiceAlive())
            {
                return new List<GroupDto>();
            }

            RestClient<GroupDto> baseClient = GetRestClient();
            return await baseClient.Get();
        }

        public async Task<GroupDto> Get(string groupName)
        {
            if(!IsServiceAlive())
            {
                return null;
            }

            RestClient<GroupDto> baseClient = GetRestClient();
            return await baseClient.Get(groupName);
        }

        public async Task<HttpResponseMessage> Post(GroupDto group)
        {
            AssertIfServiceNotAlive();

            RestClient<GroupDto> baseClient = GetRestClient();
            return await baseClient.Post(group);
        }

        public async Task<HttpResponseMessage> Put(string groupName, GroupDto group)
        {
            AssertIfServiceNotAlive();

            RestClient<GroupDto> baseClient = GetRestClient();
            return await baseClient.Put(groupName, group);
        }

        public async Task<HttpResponseMessage> Delete(string groupName)
        {
            AssertIfServiceNotAlive();

            RestClient<GroupDto> baseClient = GetRestClient();
            return await baseClient.Delete(groupName);
        }

        private RestClient<GroupDto> GetRestClient()
        {
            string resourceAddress = _baseAddress.AppendPathSegment(_resourceUri);
            var baseClient = new RestClient<GroupDto>(_httpClient, resourceAddress, _customHeaders);
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