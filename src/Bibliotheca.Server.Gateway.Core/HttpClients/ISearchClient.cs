using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public interface ISearchClient
    {
        Task<DocumentSearchResultDto<DocumentIndexDto>> Get(FilterDto filter);

        Task<DocumentSearchResultDto<DocumentIndexDto>> Get(FilterDto filter, string projectId, string branchName);

        Task<HttpResponseMessage> Post(string projectId, string branchName, IEnumerable<DocumentIndexDto> documentIndexDtos);

        Task<HttpResponseMessage> Delete(string projectId, string branchName);
    }
}
