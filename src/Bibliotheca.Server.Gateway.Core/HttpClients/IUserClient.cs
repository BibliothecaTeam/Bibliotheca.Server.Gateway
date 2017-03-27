using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public interface IUsersClient
    {
        Task<IList<UserDto>> Get();

        Task<UserDto> Get(string id);

        Task<HttpResponseMessage> Post(UserDto user);

        Task<HttpResponseMessage> Put(string id, UserDto user);

        Task<HttpResponseMessage> Delete(string id);

        Task<HttpResponseMessage> RefreshToken(string id, AccessTokenDto accessToken);
    }
}