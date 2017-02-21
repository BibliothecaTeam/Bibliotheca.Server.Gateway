using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Exceptions;
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
            var result = await _searchClient.Post(projectId, branchName, documentDtos);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new UpdateSearchIndexException("During updating search index error occurs: " + content);
            }
        }

        public async Task DeleteDocumentsAsync(string projectId, string branchName)
        {
            var result = await _searchClient.Delete(projectId, branchName);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new DeleteSearchIndexException("During deleting search index error occurs: " + content);
            }
        }
    }
}