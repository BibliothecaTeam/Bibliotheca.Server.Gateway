using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Model;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IStorageService
    {
        Task<string[]> GetProjectsPathsAsync();
        Task<string[]> GetBranchesPathsAsync(Project project);

        Task<string> ReadProjectConfigurationFileAsyc(string path);
        Task<string> ReadAllTextAsync(Project project, Branch branch, string file);
        Task<byte[]> ReadAllBytesAsync(Project project, Branch branch, string file);

        Task<ActionConfirmation> CreateProjectAsync(Project project);
        Task<ActionConfirmation> UpdateProjectAsync(string projectId, Project project);
        Task<ActionConfirmation> DeleteProjectAsync(string projectId);

        Task<ActionConfirmation<string>> UploadBranchAsync(Project project, IFormFile file);
        Task<ActionConfirmation> DeleteBranchAsync(string projectId, string branchName);
    }
}
