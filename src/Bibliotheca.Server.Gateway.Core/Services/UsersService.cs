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
        private const string _allUsersInformationCacheKey = "UsersService";

        private readonly IUsersClient _usersClient;

        private readonly IMemoryCache _memoryCache;

        public UsersService(IUsersClient usersClient, IMemoryCache memoryCache)
        {
            _usersClient = usersClient;
            _memoryCache = memoryCache;
        }

        public async Task<IList<UserDto>> GetUsersAsync()
        {
            IList<UserDto> users = null;
            if (!TryGetUsers(out users))
            {
                users = await _usersClient.Get();
                AddUsers(users);
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

            ClearCache();
        }

        public async Task UpdateUserAsync(string id, UserDto user)
        {
            var result = await _usersClient.Put(id, user);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new UpdateUserException("During updating user error occurs: " + content);
            }

            ClearCache();
        }

        public async Task DeleteUserAsync(string id)
        {
            var result = await _usersClient.Delete(id);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new DeleteUserException("During deleting user error occurs: " + content);
            }

            ClearCache();
        }

        public async Task RefreshTokenAsync(string id, AccessTokenDto accessToken)
        {
            var result = await _usersClient.RefreshToken(id, accessToken);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new UpdateUserException("During refreshing user's access token error occurs: " + content);
            }

            ClearCache();
        }

        private bool TryGetUsers(out IList<UserDto> users)
        {
            return _memoryCache.TryGetValue(_allUsersInformationCacheKey, out users);
        }

        private void AddUsers(IList<UserDto> users)
        {
            _memoryCache.Set(_allUsersInformationCacheKey, users,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }

        private void ClearCache()
        {
            _memoryCache.Remove(_allUsersInformationCacheKey);
        }
    }
}
