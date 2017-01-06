using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Indexer.Abstractions.DataTransferObjects;
using Bibliotheca.Server.Indexer.Client;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class SearchService : ISearchService
    {
        private readonly ISearchClient _searchClient;

        public SearchService(ISearchClient searchClient)
        {
            _searchClient = searchClient;
        }

        public async Task<DocumentSearchResultDto<DocumentIndexDto>> SearchAsync(FilterDto filter)
        {
            var result = await _searchClient.Get(filter);
            return result;
        }

        public async Task<DocumentSearchResultDto<DocumentIndexDto>> SearchAsync(FilterDto filter, string projectId, string branchName)
        {
            var result = await _searchClient.Get(filter, projectId, branchName);
            return result;
        }

        public async Task UploadDocumentsAsync(string projectId, string branchName, IEnumerable<DocumentIndexDto> documentDtos)
        {
            await _searchClient.Post(projectId, branchName, documentDtos);
        }

        public async Task DeleteDocumentsAsync(string projectId, string branchName)
        {
            await _searchClient.Delete(projectId, branchName);
        }
    }
}