using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Model;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class TagsService : ITagsService
    {
        private readonly IProjectsService _projectService;

        public TagsService(IProjectsService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IEnumerable<string>> GetAvailableTagsAsync()
        {
            var projects = await _projectService.GetProjectsAsync();
            var availableTags = ReadAvailableTags(projects);

            return availableTags;
        }

        private IEnumerable<string> ReadAvailableTags(IEnumerable<Project> projects)
        {
            var availableTags = new List<string>();
            foreach (var tags in projects.Select(x => x.Tags))
            {
                foreach (var tag in tags)
                {
                    if (!availableTags.Contains(tag))
                    {
                        availableTags.Add(tag);
                    }
                }
            }

            availableTags.Sort();
            return availableTags;
        }
    }
}
