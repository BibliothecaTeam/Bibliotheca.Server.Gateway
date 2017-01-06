using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;
using Bibliotheca.Server.Depository.Client;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectsClient _projectsClient;

        public ProjectsService(IProjectsClient projectsClient)
        {
            _projectsClient = projectsClient;
        }

        public async Task<IList<ProjectDto>> GetProjectsAsync()
        {
            var projects = await _projectsClient.Get();
            return projects;
        }
    }
}
