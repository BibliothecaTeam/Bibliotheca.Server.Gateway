using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IBranchesService
    {
        Task<IList<ExtendedBranchDto>> GetBranchesAsync(string projectId);

        Task<ExtendedBranchDto> GetBranchAsync(string projectId, string branchName);

        Task CreateBranchAsync(string projectId, BranchDto branch);

        Task UpdateBranchAsync(string projectId, string branchName, BranchDto branch);
        
        Task DeleteBranchAsync(string projectId, string branchName);
    }
}