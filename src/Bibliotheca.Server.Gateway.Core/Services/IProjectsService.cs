using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IProjectsService
    {
        Task<FilteredResutsDto<ProjectDto>> GetProjectsAsync(ProjectsFilterDto filter);

        Task<ProjectDto> GetProjectAsync(string projectId);

        Task CreateProjectAsync(ProjectDto project);

        Task UpdateProjectAsync(string projectId, ProjectDto project);

        Task DeleteProjectAsync(string projectId);
        
        Task<AccessTokenDto> GetProjectAccessTokenAsync(string projectId);
    }
}