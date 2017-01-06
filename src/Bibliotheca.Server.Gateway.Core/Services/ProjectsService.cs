using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;
using Bibliotheca.Server.Depository.Client;
using Microsoft.Extensions.Caching.Memory;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class ProjectsService : IProjectsService
    {
        private const string _allProjectsInformationCacheKey = "all-projects-information";

        private readonly IProjectsClient _projectsClient;

        private readonly IMemoryCache _memoryCache;

        public ProjectsService(IProjectsClient projectsClient, IMemoryCache memoryCache)
        {
            _projectsClient = projectsClient;
            _memoryCache = memoryCache;
        }

        public async Task<IList<ProjectDto>> GetProjectsAsync()
        {
            IList<ProjectDto> projects = null;
            if (!TryGetProjects(out projects))
            {
                projects = await _projectsClient.Get();
                AddProjects(projects);
            }

            return projects;
        }

        public async Task<ProjectDto> GetProjectAsync(string projectId)
        {
            var projects = await _projectsClient.Get(projectId);
            return projects;
        }

        public async Task CreateProjectAsync(ProjectDto project)
        {
            await _projectsClient.Post(project);
        }

        public async Task UpdateProjectAsync(string projectId, ProjectDto project)
        {
            await _projectsClient.Put(projectId, project);
        }

        public async Task DeleteProjectAsync(string projectId)
        {
            await _projectsClient.Delete(projectId);
        }

        public bool TryGetProjects(out IList<ProjectDto> projects)
        {
            return _memoryCache.TryGetValue(_allProjectsInformationCacheKey, out projects);
        }

        public void AddProjects(IList<ProjectDto> projects)
        {
            _memoryCache.Set(_allProjectsInformationCacheKey, projects,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }
    }
}
