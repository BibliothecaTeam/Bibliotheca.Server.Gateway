using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Microsoft.Extensions.Caching.Memory;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersClient _usersClient;

        private readonly ICacheService _cacheService;

        public UsersService(IUsersClient usersClient, ICacheService cacheService)
        {
            _usersClient = usersClient;
            _cacheService = cacheService;
        }

        public async Task<IList<UserDto>> GetUsersAsync()
        {
            IList<UserDto> users = null;
            if (!_cacheService.TryGetUsers(out users))
            {
                users = await _usersClient.Get();
                _cacheService.AddUsers(users);
            }

            return users;
        }

        public async Task<UserDto> GetUserAsync(string id)
        {
            UserDto user = null;
            try
            {
                user = await _usersClient.Get(id);
            }
            finally
            {
                if(user == null) 
                {
                    user = new UserDto 
                    {
                        Id = id,
                        Role = RoleEnumDto.Unknown
                    };
                }
            }

            return user;
        }

        public async Task CreateUserAsync(UserDto user)
        {
            var result = await _usersClient.Post(user);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new CreateUserException("During creating user error occurs: " + content);
            }

            _cacheService.ClearUsersCache();
        }

        public async Task UpdateUserAsync(string id, UserDto user)
        {
            var result = await _usersClient.Put(id, user);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new UpdateUserException("During updating user error occurs: " + content);
            }

            _cacheService.ClearUsersCache();
        }

        public async Task DeleteUserAsync(string id)
        {
            var result = await _usersClient.Delete(id);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new DeleteUserException("During deleting user error occurs: " + content);
            }

            _cacheService.ClearUsersCache();
        }

        public async Task RefreshTokenAsync(string id, AccessTokenDto accessToken)
        {
            var result = await _usersClient.RefreshToken(id, accessToken);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new UpdateUserException("During refreshing user's access token error occurs: " + content);
            }

            _cacheService.ClearUsersCache();
        }

        public async Task AddProjectToUserAsync(string id, string projectId)
        {
            var user = await _usersClient.Get(id.ToLower());
            if(user != null)
            {
                if(user.Projects == null) 
                {
                    user.Projects = new List<string>();
                }

                if(!user.Projects.Contains(projectId))
                {
                    user.Projects.Add(projectId);
                    await UpdateUserAsync(user.Id, user);
                }
            }
        }
    }
}
