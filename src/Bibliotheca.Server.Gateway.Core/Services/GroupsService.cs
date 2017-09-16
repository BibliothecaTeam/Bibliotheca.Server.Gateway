using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Microsoft.Extensions.Caching.Memory;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class GroupsService : IGroupsService
    {
        private readonly IGroupsClient _groupsClient;

        private readonly ICacheService _cacheService;

        public GroupsService(IGroupsClient groupsClient, ICacheService cacheService)
        {
            _groupsClient = groupsClient;
            _cacheService = cacheService;
        }

        public async Task<IList<GroupDto>> GetGroupsAsync()
        {
            if (!_cacheService.TryGetGroups(out IList<GroupDto> groups))
            {
                groups = await _groupsClient.Get();
                _cacheService.AddGroups(groups);
            }

            return groups;
        }

        public async Task<GroupDto> GetGroupAsync(string groupName)
        {
            var group = await _groupsClient.Get(groupName);
            return group;
        }

        public async Task CreateGroupAsync(GroupDto group)
        {
            var result = await _groupsClient.Post(group);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new CreateGroupException("During creating group error occurs: " + content);
            }

            _cacheService.ClearGroupsCache();
        }

        public async Task UpdateGroupAsync(string groupName, GroupDto group)
        {
            var result = await _groupsClient.Put(groupName, group);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new UpdateGroupException("During updating group error occurs: " + content);
            }

            _cacheService.ClearGroupsCache();
        }

        public async Task DeleteGroupAsync(string groupName)
        {
            var result = await _groupsClient.Delete(groupName);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new DeleteGroupException("During deleting group error occurs: " + content);
            }

            _cacheService.ClearGroupsCache();
        }
    }
}