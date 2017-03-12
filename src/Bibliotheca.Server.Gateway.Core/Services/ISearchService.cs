using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface ISearchService
    {
        Task<DocumentSearchResultDto<DocumentIndexDto>> SearchAsync(FilterDto filter);

        Task<DocumentSearchResultDto<DocumentIndexDto>> SearchAsync(FilterDto filter, string projectId, string branchName);

        Task UploadDocumentsAsync(string projectId, string branchName, IEnumerable<DocumentIndexDto> documentDtos);

        Task RefreshIndexAsync(string projectId, string branchName);

        Task<DataTransferObjects.IndexStatusDto> GetRefreshIndexStatusAsync(string projectId, string branchName);

        Task DeleteDocumentsAsync(string projectId, string branchName);
    }
}