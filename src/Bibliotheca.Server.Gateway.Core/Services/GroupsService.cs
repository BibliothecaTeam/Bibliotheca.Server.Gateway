using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Model;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class GroupsService : IGroupsService
    {
        private readonly IProjectsService _projectService;

        public GroupsService(IProjectsService projectService)
        {
            _projectService = projectService;
        }

        public async Task<IEnumerable<string>> GetAvailableGroupsAsync()
        {
            var projects = await _projectService.GetProjectsAsync();
            var availableGroup = ReadAvailableGroups(projects);

            return availableGroup;
        }

        private IEnumerable<string> ReadAvailableGroups(IEnumerable<Project> projects)
        {
            var availableGroups = new List<string>();
            foreach (var group in projects.Select(x => x.Group))
            {
                if (!availableGroups.Contains(group))
                {
                    availableGroups.Add(group);
                }
            }

            return availableGroups;
        }
    }
}
