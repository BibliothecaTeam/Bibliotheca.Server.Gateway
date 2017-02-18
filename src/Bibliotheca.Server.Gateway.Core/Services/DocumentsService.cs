using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;
using Bibliotheca.Server.Depository.Client;
using Microsoft.Extensions.Caching.Memory;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IDocumentsClient _documentsClient;

        private readonly IMemoryCache _memoryCache;

        public DocumentsService(IDocumentsClient documentsClient, IMemoryCache memoryCache)
        {
            _documentsClient = documentsClient;
            _memoryCache = memoryCache;
        }

        public async Task<IList<BaseDocumentDto>> GetDocumentsAsync(string projectId, string branchName)
        {
            var documentsDto = await _documentsClient.Get(projectId, branchName);
            return documentsDto;
        }

        public async Task<DocumentDto> GetDocumentAsync(string projectId, string branchName, string fileUri)
        {
            DocumentDto documentDto = null;
            string cacheKey = GetCacheKey(projectId, branchName, fileUri);

            if (!_memoryCache.TryGetValue(cacheKey, out documentDto))
            {
                documentDto = await _documentsClient.Get(projectId, branchName, fileUri);

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                _memoryCache.Set(cacheKey, documentDto, cacheEntryOptions);
            }

            return documentDto;
        }

        public async Task CreateDocumentAsync(string projectId, string branchName, DocumentDto document)
        {
            await _documentsClient.Post(projectId, branchName, document);
        }

        public async Task UpdateDocumentAsync(string projectId, string branchName, string fileUri, DocumentDto document)
        {
            await _documentsClient.Put(projectId, branchName, fileUri, document);
        }

        public async Task DeleteDocumentAsync(string projectId, string branchName, string fileUri)
        {
            await _documentsClient.Delete(projectId, branchName, fileUri);
        }

        private string GetCacheKey(string projectId, string branchName, string fileUri)
        {
            return $"DocumentsService#{projectId}#{branchName}#{fileUri}";
        }
    }
}