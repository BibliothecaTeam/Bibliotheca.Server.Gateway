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
        private const string _tagsCacheKey = "TagsService";

        private readonly IProjectsClient _projectsClient;

        private readonly IMemoryCache _memoryCache;

        public TagsService(IProjectsClient projectsClient, IMemoryCache memoryCache)
        {
            _projectsClient = projectsClient;
            _memoryCache = memoryCache;
        }

        public async Task<IList<string>> GetAvailableTagsAsync()
        {
            IList<string> tags = null;
            if (!TryGetTags(out tags))
            {
                var projects = await _projectsClient.Get();
                tags = new List<string>();
                foreach (var project in projects)
                {
                    project.Tags.ForEach(x => tags.Add(x));
                }

                tags = tags.OrderBy(x => x).Distinct().ToList();
                AddTags(tags);
            }

            return tags;
        }

        public bool TryGetTags(out IList<string> tags)
        {
            return _memoryCache.TryGetValue(_tagsCacheKey, out tags);
        }

        public void AddTags(IList<string> tags)
        {
            _memoryCache.Set(_tagsCacheKey, tags,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }
    }
}