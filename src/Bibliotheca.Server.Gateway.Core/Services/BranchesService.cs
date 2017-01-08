using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;
using Bibliotheca.Server.Depository.Client;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class BranchesService : IBranchesService
    {
        private readonly IBranchesClient _branchesClient;

        public BranchesService(IBranchesClient branchesClient)
        {
            _branchesClient = branchesClient;
        }

        public async Task<IList<BranchDto>> GetBranchesAsync(string projectId)
        {
            var branches = await _branchesClient.Get(projectId);
            return branches;
        }

        public async Task<BranchDto> GetBranchAsync(string projectId, string branchName)
        {
            var branch = await _branchesClient.Get(projectId, branchName);
            return branch;
        }

        public async Task CreateBranchAsync(string projectId, BranchDto branch)
        {
            await _branchesClient.Post(projectId, branch);
        }

        public async Task UpdateBranchAsync(string projectId, string branchName, BranchDto branch)
        {
            await _branchesClient.Put(projectId, branchName, branch);
        }
        
        public async Task DeleteBranchAsync(string projectId, string branchName)
        {
            await _branchesClient.Delete(projectId, branchName);
        }
    }
}