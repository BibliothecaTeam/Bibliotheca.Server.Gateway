using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Microsoft.Extensions.Caching.Memory;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class TagsService : ITagsService
    {
        private readonly IProjectsClient _projectsClient;

        private readonly ICacheService _cacheService;

        public TagsService(IProjectsClient projectsClient, ICacheService cacheService)
        {
            _projectsClient = projectsClient;
            _cacheService = cacheService;
        }

        public async Task<IList<string>> GetAvailableTagsAsync()
        {
            IList<string> tags = null;
            if (!_cacheService.TryGetTags(out tags))
            {
                var projects = await _projectsClient.Get();
                tags = new List<string>();
                foreach (var project in projects)
                {
                    project.Tags.ForEach(x => tags.Add(x));
                }

                tags = tags.OrderBy(x => x).Distinct().ToList();
                _cacheService.AddTags(tags);
            }

            return tags;
        }
    }
}