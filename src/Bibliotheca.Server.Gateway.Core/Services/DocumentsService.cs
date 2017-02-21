using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;
using Bibliotheca.Server.Depository.Client;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Microsoft.Extensions.Caching.Memory;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IDocumentsClient _documentsClient;

        private readonly IBranchesService _branchService;

        private readonly IMemoryCache _memoryCache;

        public DocumentsService(IDocumentsClient documentsClient, IBranchesService branchService, IMemoryCache memoryCache)
        {
            _documentsClient = documentsClient;
            _branchService = branchService;
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
            var result = await _documentsClient.Post(projectId, branchName, document);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new CreateDocumentException("During creating document error occurs: " + content);
            }
        }

        public async Task UpdateDocumentAsync(string projectId, string branchName, string fileUri, DocumentDto document)
        {
            var result = await _documentsClient.Put(projectId, branchName, fileUri, document);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new UpdateDocumentException("During updating document error occurs: " + content);
            }
        }

        public async Task UploadBranchAsync(string projectId, string branchName, Stream body)
        {
            using (var zipArchive = new ZipArchive(body))
            {
                var entries = zipArchive.Entries;
                foreach (var entry in entries)
                {
                    bool isDirectory = entry.FullName.EndsWith("/");
                    if (!isDirectory)
                    {
                        byte[] content = null;
                        using (var stream = entry.Open())
                        using (MemoryStream ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            content = ms.ToArray();
                        }
                        
                        var fileUri = GetPathWithoutRootDirectory(entry.FullName);
                        if(fileUri == "mkdocs.yml")
                        {
                            await CreateBranchAsync(projectId, branchName, content);
                        }
                        else
                        {
                            await UploadDocumnentAsync(projectId, branchName, fileUri, content);
                        }
                    }
                }
            }
        }

        private async Task CreateBranchAsync(string projectId, string branchName, byte[] content)
        {
            string mkdocsContent = Encoding.UTF8.GetString(content);

            var branch = new BranchDto
            {
                Name = branchName,
                MkDocsYaml = mkdocsContent
            };
            await _branchService.CreateBranchAsync(projectId, branch);
        }

        private async Task UploadDocumnentAsync(string projectId, string branchName, string fileUri, byte[] content)
        {
            var fileName = Path.GetFileName(fileUri);

            var document = new DocumentDto
            {
                Content = content,
                Uri = fileUri,
                Name = fileName
            };

            await CreateDocumentAsync(projectId, branchName, document);
        }

        private string GetPathWithoutRootDirectory(string path)
        {
            var parts = path.Split('/');
            string newPath = string.Empty;
            for(int i = 1; i < parts.Length; ++i)
            {
                newPath = Path.Combine(newPath, parts[i]);
            }

            return newPath;
        }

        public async Task DeleteDocumentAsync(string projectId, string branchName, string fileUri)
        {
            var result = await _documentsClient.Delete(projectId, branchName, fileUri);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new DeleteDocumentException("During deleting document error occurs: " + content);
            }
        }

        private string GetCacheKey(string projectId, string branchName, string fileUri)
        {
            return $"DocumentsService#{projectId}#{branchName}#{fileUri}";
        }
    }
}