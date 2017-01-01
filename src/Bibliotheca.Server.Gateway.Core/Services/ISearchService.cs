using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Model;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface ISearchService
    {
        Task<DocumentSearchResult<DocumentIndex>> SearchAsync(Filter filter);

        Task<DocumentSearchResult<DocumentIndex>> SearchAsync(Filter filter, string projectId, string branchName);

        Task CreateOrUpdateIndexAsync();

        Task DeleteIndexIfExistsAsync();

        Task DeleteDocumemntsAsync(string projectId, string branchName);

        Task ReindexDocumentAsync(string projectId, string branchName, IEnumerable<DocumentIndex> documentDtos);

        DocumentIndex PrepareDocument(string url, Project project, string branchName, string content);
    }
}
