using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class UsersClient : IUsersClient
    {
        private const string _resourceUri = "users";

        private readonly string _baseAddress;

        private readonly IDictionary<string, StringValues> _customHeaders;

        public UsersClient(string baseAddress, IDictionary<string, StringValues> customHeaders)
        {
            _baseAddress = baseAddress;
            _customHeaders = customHeaders;
        }

        public async Task<IList<UserDto>> Get()
        {
            RestClient<UserDto> baseClient = GetRestClient();
            return await baseClient.Get();
        }

        public async Task<UserDto> Get(string id)
        {
            RestClient<UserDto> baseClient = GetRestClient();
            return await baseClient.Get(id);
        }

        public async Task<HttpResponseMessage> Post(UserDto user)
        {
            RestClient<UserDto> baseClient = GetRestClient();
            return await baseClient.Post(user);
        }

        public async Task<HttpResponseMessage> Put(string id, UserDto user)
        {
            RestClient<UserDto> baseClient = GetRestClient();
            return await baseClient.Put(id, user);
        }

        public async Task<HttpResponseMessage> Delete(string id)
        {
            RestClient<UserDto> baseClient = GetRestClient();
            return await baseClient.Delete(id);
        }

        private RestClient<UserDto> GetRestClient()
        {
            string resourceAddress = Path.Combine(_baseAddress, _resourceUri);
            var baseClient = new RestClient<UserDto>(resourceAddress, _customHeaders);
            return baseClient;
        }
    }
}