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
        private readonly IProjectsClient _projectsClient;

        private readonly ICacheService _cacheService;

        public GroupsService(IProjectsClient projectsClient, ICacheService cacheService)
        {
            _projectsClient = projectsClient;
            _cacheService = cacheService;
        }

        public async Task<IList<string>> GetAvailableGroupsAsync()
        {
            IList<string> groups = null;
            if (!_cacheService.TryGetGroups(out groups))
            {
                var projects = await _projectsClient.Get();

                groups = new List<string>();
                foreach (var project in projects)
                {
                    groups.Add(project.Group);
                }

                groups = groups.OrderBy(x => x).Distinct().ToList();
                _cacheService.AddGroups(groups);
            }

            return groups;
        }
    }
}