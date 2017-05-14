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
    public class ProjectsService : IProjectsService
    {
        private readonly IProjectsClient _projectsClient;

        private readonly IUsersClient _usersClient;

        private readonly ICacheService _cacheService;

        public ProjectsService(IProjectsClient projectsClient, IUsersClient usersClient, ICacheService cacheService)
        {
            _projectsClient = projectsClient;
            _usersClient = usersClient;
            _cacheService = cacheService;
        }

        public async Task<FilteredResutsDto<ProjectDto>> GetProjectsAsync(ProjectsFilterDto filter, string userId)
        {
            IList<ProjectDto> projects = null;
            if (!_cacheService.TryGetProjects(out projects))
            {
                var projectsTask = _projectsClient.Get();
                var usersTask = _usersClient.Get();

                projects = await projectsTask;
                CleanProjectsAccessTokens(projects);

                var users = await usersTask;
                AddOwnersToProjects(projects, users);

                _cacheService.AddProjectsToCache(projects);
            }

            IEnumerable<ProjectDto> query = projects;
            query = FilterByName(filter, query);
            query = FilterByDescription(filter, query);
            query = FilterByGroups(filter, query);
            query = FilterByTags(filter, query);
            query = FilterByLimitedAccess(filter, query, userId);

            var allResults = query.Count();

            query = query.OrderBy(x => x.Name);
            if (filter.Limit > 0)
            {
                query = query.Skip(filter.Page * filter.Limit).Take(filter.Limit);
            }

            var filteredResults = new FilteredResutsDto<ProjectDto>
            {
                Results = query,
                AllResults = allResults
            };

            return filteredResults;
        }

        public async Task<ProjectDto> GetProjectAsync(string projectId)
        {
            var project = await _projectsClient.Get(projectId);
            if(project != null)
            {
                project.AccessToken = string.Empty;
            }
            
            return project;
        }

        public async Task CreateProjectAsync(ProjectDto project)
        {
            var result = await _projectsClient.Post(project);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new CreateProjectException("During creating project error occurs: " + content);
            }

            _cacheService.ClearProjectsCache();
            _cacheService.ClearGroupsCache();
            _cacheService.ClearTagsCache();
        }

        public async Task UpdateProjectAsync(string projectId, ProjectDto project)
        {
            var result = await _projectsClient.Put(projectId, project);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new UpdateProjectException("During updating project error occurs: " + content);
            }

            _cacheService.ClearProjectsCache();
            _cacheService.ClearGroupsCache();
            _cacheService.ClearTagsCache();
        }

        public async Task DeleteProjectAsync(string projectId)
        {
            var result = await _projectsClient.Delete(projectId);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new DeleteProjectException("During deleting project error occurs: " + content);
            }

            _cacheService.ClearProjectsCache();
            _cacheService.ClearGroupsCache();
            _cacheService.ClearTagsCache();
        }

        public async Task<AccessTokenDto> GetProjectAccessTokenAsync(string projectId)
        {
            var project = await _projectsClient.Get(projectId);
            if(project == null)
            {
                throw new ProjectNotFoundException($"Project '{projectId}' not found.");
            }

            var accessToken = new AccessTokenDto
            {
                AccessToken = project.AccessToken
            };

            return accessToken;
        }

        private static void AddOwnersToProjects(IList<ProjectDto> projects, IList<UserDto> users)
        {
            foreach (var project in projects)
            {
                var owners = users.Where(x => x.Role == RoleEnumDto.Administrator || (x.Projects != null && x.Projects.Contains(project.Id)));
                project.Owners = owners.Select(x => x.Id).ToList();
            }
        }

        private static void CleanProjectsAccessTokens(IList<ProjectDto> projects)
        {
            foreach (var project in projects)
            {
                project.AccessToken = string.Empty;
            }
        }

        private static IEnumerable<ProjectDto> FilterByTags(ProjectsFilterDto filter, IEnumerable<ProjectDto> query)
        {
            if (filter.Tags != null)
            {
                query = query.Where(t2 => filter.Tags.Any(t1 => t2.Tags.Contains(t1)));
            }

            return query;
        }

        private static IEnumerable<ProjectDto> FilterByGroups(ProjectsFilterDto filter, IEnumerable<ProjectDto> query)
        {
            if (filter.Groups != null)
            {
                var groupsNormalized = new List<string>(filter.Groups.Count);
                foreach (var item in filter.Groups)
                {
                    groupsNormalized.Add(item.ToUpper());
                }

                query = query.Where(x => groupsNormalized.Contains(x.Group.ToUpper()));
            }

            return query;
        }

        private static IEnumerable<ProjectDto> FilterByName(ProjectsFilterDto filter, IEnumerable<ProjectDto> query)
        {
            if (!string.IsNullOrWhiteSpace(filter.Query))
            {
                string filterQueryNormalized = GetNormalizedQuery(filter);
                query = query.Where(x => x.Name.ToUpper().Contains(filterQueryNormalized));
            }

            return query;
        }

        private static IEnumerable<ProjectDto> FilterByDescription(ProjectsFilterDto filter, IEnumerable<ProjectDto> query)
        {
            if (!string.IsNullOrWhiteSpace(filter.Query))
            {
                string filterQueryNormalized = GetNormalizedQuery(filter);
                query = query.Where(x => x.Description.ToUpper().Contains(filterQueryNormalized));
            }

            return query;
        }
        
        private static IEnumerable<ProjectDto> FilterByLimitedAccess(ProjectsFilterDto filter, IEnumerable<ProjectDto> query, string userId)
        {
            query = query.Where(x => 
            (
                !x.IsAccessLimited ||
                (
                    x.IsAccessLimited && 
                    ((x.ContactPeople != null && x.ContactPeople.Any(y => y.Email == userId)) || x.Owners.Any(o => o == userId)))
                )
            );
            return query;
        }

        private static string GetNormalizedQuery(ProjectsFilterDto filter)
        {
            var filterQueryNormalized = $" {filter.Query.ToUpper().Trim()}";
            if (filterQueryNormalized.Last() == '*')
            {
                filterQueryNormalized = filterQueryNormalized.TrimEnd('*');
            }
            else
            {
                filterQueryNormalized = $"{filterQueryNormalized} ";
            }

            return filterQueryNormalized;
        }
    }
}
