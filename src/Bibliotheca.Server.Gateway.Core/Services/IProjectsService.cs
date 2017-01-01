using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Model;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IProjectsService
    {
        Task<IEnumerable<Project>> GetProjectsAsync();

        Task<FilteredResuts<Project>> GetProjectsAsync(ProjectsFilter filter);

        Task<Project> GetProjectAsync(string projectId);

        Task<ActionConfirmation<Project>> CreateProjectAsync(Project project);

        Task<ActionConfirmation> UpdateProjectAsync(string projectId, Project project);

        Task<ActionConfirmation> DeleteProjectAsync(string projectId);

        Task<IEnumerable<Branch>> GetBranchesAsync();

        Task<IEnumerable<Branch>> GetBranchesAsync(string projectId);

        Task<Branch> GetBranchAsync(string projectId, string branch);

        Task<ActionConfirmation<Branch>> UploadBranchAsync(Project project, IFormFile file);

        Task<ActionConfirmation> DeleteBranchAsync(string projectId, string branchName);

        void DestroyCache();
    }
}