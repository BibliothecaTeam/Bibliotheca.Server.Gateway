using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IBranchesService
    {
        Task<IList<BranchDto>> GetBranchesAsync(string projectId);

        Task<BranchDto> GetBranchAsync(string projectId, string branchName);

        Task CreateBranchAsync(string projectId, BranchDto branch);

        Task UpdateBranchAsync(string projectId, string branchName, BranchDto branch);
        
        Task DeleteBranchAsync(string projectId, string branchName);
    }
}