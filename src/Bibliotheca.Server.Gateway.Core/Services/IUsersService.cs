using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IUsersService
    {
        Task<IList<UserDto>> GetUsersAsync();

        Task<UserDto> GetUserAsync(string id);

        Task CreateUserAsync(UserDto user);

        Task UpdateUserAsync(string id, UserDto user);

        Task DeleteUserAsync(string id);
    }
}