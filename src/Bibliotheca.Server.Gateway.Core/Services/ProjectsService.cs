using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Model;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Bibliotheca.Server.Gateway.Core.Utilities;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class ProjectsService : IProjectsService
    {
        private const string _allProjectsInformationCacheKey = "all-projects-information";
        private readonly IMkDocsService _mkDocsService;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IStorageService _storageService;

        public ProjectsService(IMkDocsService mkDocsService, ILoggerFactory loggerFactory, IMemoryCache memoryCache, IStorageService storageService)
        {
            _mkDocsService = mkDocsService;
            _logger = loggerFactory.CreateLogger<ProjectsService>();
            _memoryCache = memoryCache;
            _storageService = storageService;
        }

        public async Task<ActionConfirmation<Project>> CreateProjectAsync(Project project)
        {
            var actionConfirmation = await _storageService.CreateProjectAsync(project);
            if(actionConfirmation.WasSuccess)
            {
                DestroyCache();

                var createdProject = await GetProjectAsync(project.Id);
                return ActionConfirmation<Project>.CreateSuccessfull(createdProject);
            }

            return ActionConfirmation<Project>.CreateError(actionConfirmation.Message);
        }

        public async Task<ActionConfirmation> UpdateProjectAsync(string projectId, Project project)
        {
            var actionConfirmation = await _storageService.UpdateProjectAsync(projectId, project);
            if (actionConfirmation.WasSuccess)
            {
                DestroyCache();
            }

            return actionConfirmation;
        }

        public async Task<IEnumerable<Project>> GetProjectsAsync()
        {
            IEnumerable<Project> projects;
            if (!_memoryCache.TryGetValue(_allProjectsInformationCacheKey, out projects))
            {
                projects = await ReadAllProjectsAsync();
                AddToCache(projects);
            }

            return projects;
        }

        public async Task<FilteredResuts<Project>> GetProjectsAsync(ProjectsFilter filter)
        {
            var projects = await GetProjectsAsync();

            var query = projects;
            query = FilterByName(filter, query);
            query = FilterByGroups(filter, query);
            query = FilterByTags(filter, query);

            var allResults = query.Count();

            query = query.OrderBy(x => x.Name);
            if (filter.Limit > 0)
            {
                query = query.Skip(filter.Page * filter.Limit).Take(filter.Limit);
            }

            var filteredResults = new FilteredResuts<Project>
            {
                Results = query,
                AllResults = allResults
            };

            return filteredResults;
        }

        private static IEnumerable<Project> FilterByTags(ProjectsFilter filter, IEnumerable<Project> query)
        {
            if (filter.Tags != null)
            {
                var tagsNormalized = new List<string>(filter.Tags.Count);
                foreach (var item in filter.Tags)
                {
                    tagsNormalized.Add(item.ToUpper());
                }

                query = query.Where(t2 => tagsNormalized.Any(t1 => t2.TagsNormmalized.Contains(t1)));
            }

            return query;
        }

        private static IEnumerable<Project> FilterByGroups(ProjectsFilter filter, IEnumerable<Project> query)
        {
            if (filter.Groups != null)
            {
                var groupsNormalized = new List<string>(filter.Groups.Count);
                foreach (var item in filter.Groups)
                {
                    groupsNormalized.Add(item.ToUpper());
                }

                query = query.Where(x => groupsNormalized.Contains(x.GroupNormalized));
            }

            return query;
        }

        private static IEnumerable<Project> FilterByName(ProjectsFilter filter, IEnumerable<Project> query)
        {
            if (!string.IsNullOrWhiteSpace(filter.Query))
            {
                var filterQueryNormalized = filter.Query.ToUpper();
                query = query.Where(x => x.NormalizedName.Contains(filterQueryNormalized));
            }

            return query;
        }

        public async Task<Project> GetProjectAsync(string projectId)
        {
            var projects = await GetProjectsAsync();
            var project = projects.FirstOrDefault(x => x.Id == projectId);
            if (project == null)
            {
                throw new ProjectNotFoundException($"Project '{projectId}' not exists.");
            }

            return project;
        }

        public async Task<ActionConfirmation> DeleteProjectAsync(string projectId)
        {
            var actionConfirmation = await _storageService.DeleteProjectAsync(projectId);
            if(actionConfirmation.WasSuccess)
            {
                DestroyCache();
            }

            return actionConfirmation;
        }

        public async Task<Branch> GetBranchAsync(string projectId, string branch)
        {
            var project = await GetProjectAsync(projectId);
            var branchInfo = project.Branches.FirstOrDefault(x => x.Name == branch);
            if (branchInfo == null)
            {
                throw new BranchNotFoundException($"Branch '{branch}' not exists.");
            }

            return branchInfo;
        }

        public async Task<IEnumerable<Branch>> GetBranchesAsync(string projectId)
        {
            var project = await GetProjectAsync(projectId);
            return project.Branches;
        }

        public async Task<IEnumerable<Branch>> GetBranchesAsync()
        {
            var branches = new List<Branch>();
            var projects = await GetProjectsAsync();
            foreach(var project in projects)
            {
                if (project.Branches != null && project.Branches.Count > 0)
                {
                    branches.AddRange(project.Branches);
                }
            }

            return branches;
        }

        public async Task<ActionConfirmation<Branch>> UploadBranchAsync(Project project, IFormFile file)
        {
            var actionConfirmation = await _storageService.UploadBranchAsync(project, file);
            if (actionConfirmation.WasSuccess)
            {
                DestroyCache();

                var branch = await GetBranchAsync(project.Id, actionConfirmation.ObjectData);
                return ActionConfirmation<Branch>.CreateSuccessfull(branch);
            }

            return ActionConfirmation<Branch>.CreateError(actionConfirmation.Message);
        }

        public async Task<ActionConfirmation> DeleteBranchAsync(string projectId, string branchName)
        {
            var actionConfirmation = await _storageService.DeleteBranchAsync(projectId, branchName);
            if(actionConfirmation.WasSuccess)
            {
                DestroyCache();
            }

            return actionConfirmation;
        }

        public void DestroyCache()
        {
            _memoryCache.Remove(_allProjectsInformationCacheKey);
        }

        private void AddToCache(IEnumerable<Project> projects)
        {
            _memoryCache.Set(_allProjectsInformationCacheKey, projects,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }

        private async Task<IEnumerable<Project>> ReadAllProjectsAsync()
        {
            List<Project> projects = new List<Project>();
            string[] projectsPaths = await _storageService.GetProjectsPathsAsync();

            foreach (var projectsPath in projectsPaths)
            {
                Project project = await GetProjectConfigurationAsync(projectsPath);

                if (!IsValidConfiguration(project))
                {
                    _logger.LogWarning($"Project in path: '{project.Url}' contains not proper configuration file.");
                    break;
                }

                await ReadBranchesInformation(project);
                projects.Add(project);
            }

            return projects.OrderBy(x => x.Name).ToList();
        }

        private async Task<Project> GetProjectConfigurationAsync(string projectPath)
        {
            string text = await _storageService.ReadProjectConfigurationFileAsyc(projectPath);

            var project = JsonConvert.DeserializeObject<Project>(text);
            project.Url = projectPath;
            project.Id = PathUtility.GetLastDirectory(projectPath);

            foreach(var item in project.Tags)
            {
                project.TagsNormmalized.Add(item.ToUpper());
            }

            return project;
        }

        private async Task<Project> ReadBranchesInformation(Project project)
        {
            string[] branchPaths = await _storageService.GetBranchesPathsAsync(project);
            foreach (var branchPath in branchPaths)
            {
                Branch branch = await GetBranchInformationAsync(project, branchPath);
                project.Branches.Add(branch);
            }

            return project;
        }

        private async Task<Branch> GetBranchInformationAsync(Project project, string directoryPath)
        {
            Branch branch = new Branch();
            branch.Project = project;
            branch.ProjectId = project.Id;
            branch.Url = directoryPath;
            branch.Name = PathUtility.GetLastDirectory(directoryPath);

            if (project.DefaultBranch == branch.Name)
            {
                project.DefaultBranchObject = branch;
            }

            await _mkDocsService.GetBranchConfigurationAsync(project, branch);
            return branch;
        }

        private bool IsValidConfiguration(Project project)
        {
            if (string.IsNullOrWhiteSpace(project.Name) || string.IsNullOrWhiteSpace(project.Id))
            {
                return false;
            }

            return true;
        }
    }
}
