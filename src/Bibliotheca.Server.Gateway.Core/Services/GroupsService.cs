using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Microsoft.Extensions.Caching.Memory;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class GroupsService : IGroupsService
    {
        private const string _groupsCacheKey = "GroupsService";

        private readonly IProjectsClient _projectsClient;

        private readonly IMemoryCache _memoryCache;

        public GroupsService(IProjectsClient projectsClient, IMemoryCache memoryCache)
        {
            _projectsClient = projectsClient;
            _memoryCache = memoryCache;
        }

        public async Task<IList<string>> GetAvailableGroupsAsync()
        {
            IList<string> groups = null;
            if (!TryGetGroups(out groups))
            {
                var projects = await _projectsClient.Get();

                groups = new List<string>();
                foreach (var project in projects)
                {
                    groups.Add(project.Group);
                }

                groups = groups.OrderBy(x => x).Distinct().ToList();
                AddGroups(groups);
            }

            return groups;
        }

        public bool TryGetGroups(out IList<string> tags)
        {
            return _memoryCache.TryGetValue(_groupsCacheKey, out tags);
        }

        public void AddGroups(IList<string> tags)
        {
            _memoryCache.Set(_groupsCacheKey, tags,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }
    }
}