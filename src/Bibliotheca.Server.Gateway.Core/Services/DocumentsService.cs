using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Microsoft.Extensions.Caching.Memory;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IDocumentsClient _documentsClient;

        private readonly IBranchesService _branchService;

        private readonly IProjectsService _projectsService;

        private readonly ICacheService _cacheService;

        public DocumentsService(
            IDocumentsClient documentsClient, 
            IBranchesService branchService, 
            IProjectsService projectsService,
            ICacheService cacheService)
        {
            _documentsClient = documentsClient;
            _branchService = branchService;
            _projectsService = projectsService;
            _cacheService = cacheService;
        }

        public async Task<IList<BaseDocumentDto>> GetDocumentsAsync(string projectId, string branchName)
        {
            var documentsDto = await _documentsClient.Get(projectId, branchName);
            return documentsDto;
        }

        public async Task<DocumentDto> GetDocumentAsync(string projectId, string branchName, string fileUri)
        {
            DocumentDto documentDto = null;

            if(branchName == null || branchName == "undefined") 
            {
                branchName = await GetDefaultBranch(projectId);
            }

            if(fileUri == null || fileUri == "undefined")
            {
                fileUri = await GetDefaultFileUri(projectId, branchName);
            }

            if (!_cacheService.TryGetDocument(projectId, branchName, fileUri, out documentDto))
            {
                documentDto = await _documentsClient.Get(projectId, branchName, fileUri);
                _cacheService.AddDocument(projectId, branchName, fileUri, documentDto);
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

            var fileUri = document.Uri.Replace("/", ":");
            _cacheService.ClearDocumentCache(projectId, branchName, fileUri);
        }

        public async Task UpdateDocumentAsync(string projectId, string branchName, string fileUri, DocumentDto document)
        {
            var result = await _documentsClient.Put(projectId, branchName, fileUri, document);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new UpdateDocumentException("During updating document error occurs: " + content);
            }

            _cacheService.ClearDocumentCache(projectId, branchName, fileUri);
        }

        public async Task UploadBranchAsync(string projectId, string branchName, Stream body)
        {
            byte[] mkdocsContent = null;
            Dictionary<string, byte[]> filesContent = new Dictionary<string, byte[]>();

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
                            mkdocsContent = content;
                        }
                        else
                        {
                            filesContent.Add(fileUri, content);
                        }
                    }
                }
            }

            if(mkdocsContent == null)
            {
                throw new BranchConfigurationFileNotExistsException("Zip file have to contains mkdocs.yml file.");
            }

            await CreateBranchAsync(projectId, branchName, mkdocsContent);
            foreach(var item in filesContent)
            {
                await UploadDocumnentAsync(projectId, branchName, item.Key, item.Value);
            }
        }

        public async Task DeleteDocumentAsync(string projectId, string branchName, string fileUri)
        {
            var result = await _documentsClient.Delete(projectId, branchName, fileUri);
            if(!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new DeleteDocumentException("During deleting document error occurs: " + content);
            }

            _cacheService.ClearDocumentCache(projectId, branchName, fileUri);
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

        private async Task<string> GetDefaultBranch(string projectId)
        {
            var project = await _projectsService.GetProjectAsync(projectId);
            if(project == null)
            {
                throw new ProjectNotFoundException($"I cannot find default branch. Project '{projectId}' not found.");
            }

            return project.DefaultBranch;
        }

        private async Task<string> GetDefaultFileUri(string projectId, string branchName)
        {
            var branch = await _branchService.GetBranchAsync(projectId, branchName);
            if(branch == null)
            {
                throw new BranchNotFoundException($"I cannot calculate default file uri. Branch '' not found.");
            }
            
            var fileUri = $"{branch.DocsDir}:index.md";
            return fileUri;
        }
    }
}