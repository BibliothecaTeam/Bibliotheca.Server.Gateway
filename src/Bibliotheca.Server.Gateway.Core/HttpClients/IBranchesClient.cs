using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public interface IBranchesClient
    {
        Task<IList<BranchDto>> Get(string projectId);

        Task<BranchDto> Get(string projectId, string branchName);

        Task<HttpResponseMessage> Post(string projectId, BranchDto branch);

        Task<HttpResponseMessage> Put(string projectId, string branchName, BranchDto branch);

        Task<HttpResponseMessage> Delete(string projectId, string branchName);
    }
}