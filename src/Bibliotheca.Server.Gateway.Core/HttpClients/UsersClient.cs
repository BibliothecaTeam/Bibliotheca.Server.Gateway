using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class UsersClient : IUsersClient
    {
        private const string _resourceUri = "users";

        private readonly string _baseAddress;

        private readonly IDictionary<string, StringValues> _customHeaders;

        private readonly HttpClient _httpClient;

        public UsersClient(string baseAddress, IDictionary<string, StringValues> customHeaders, HttpClient httpClient)
        {
            _baseAddress = baseAddress;
            _customHeaders = customHeaders;
            _httpClient = httpClient;
        }

        public async Task<IList<UserDto>> Get()
        {
            if(!IsServiceAlive())
            {
                return new List<UserDto>();
            }

            RestClient<UserDto> baseClient = GetRestClient();
            return await baseClient.Get();
        }

        public async Task<UserDto> Get(string id)
        {
            if(!IsServiceAlive())
            {
                return null;
            }

            RestClient<UserDto> baseClient = GetRestClient();
            return await baseClient.Get(id);
        }

        public async Task<HttpResponseMessage> Post(UserDto user)
        {
            AssertIfServiceNotAlive();

            RestClient<UserDto> baseClient = GetRestClient();
            return await baseClient.Post(user);
        }

        public async Task<HttpResponseMessage> Put(string id, UserDto user)
        {
            AssertIfServiceNotAlive();

            RestClient<UserDto> baseClient = GetRestClient();
            return await baseClient.Put(id, user);
        }

        public async Task<HttpResponseMessage> Delete(string id)
        {
            AssertIfServiceNotAlive();

            RestClient<UserDto> baseClient = GetRestClient();
            return await baseClient.Delete(id);
        }

        public async Task<HttpResponseMessage> RefreshToken(string id, AccessTokenDto accessToken)
        {
            AssertIfServiceNotAlive();

            string resourceAddress = Path.Combine(_baseAddress, _resourceUri);
            var baseClient = new RestClient<AccessTokenDto>(_httpClient, resourceAddress, _customHeaders, "refreshToken");
            return await baseClient.Put(id, accessToken);
        }

        private RestClient<UserDto> GetRestClient()
        {      
            string resourceAddress = Path.Combine(_baseAddress, _resourceUri);
            var baseClient = new RestClient<UserDto>(_httpClient, resourceAddress, _customHeaders);
            return baseClient;
        }

        private void AssertIfServiceNotAlive()
        {
            if(!IsServiceAlive()) 
            {
                throw new ServiceNotAvailableException($"Microservice with tag 'authorization' is not running!");
            }
        }

        private bool IsServiceAlive()
        {
            return !string.IsNullOrWhiteSpace(_baseAddress);
        }
    }
}