using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using Newtonsoft.Json;
using System.IO.Compression;
using Microsoft.AspNetCore.Http;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.Gateway.Core.Model;
using Bibliotheca.Server.Gateway.Core.Utilities;
using Bibliotheca.Server.Gateway.Core.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class AzureStorageService : IStorageService
    {
        private readonly IOptions<ApplicationParameters> _applicationParameters;
        private readonly ISearchService _searchService;

        public AzureStorageService(IOptions<ApplicationParameters> applicationParameters, ISearchService searchService)
        {
            _applicationParameters = applicationParameters;
            _searchService = searchService;
        }

        public async Task<string[]> GetProjectsPathsAsync()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.Value.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            BlobContinuationToken continuationToken = null;
            ContainerResultSegment resultSegment = null;

            List<string> containers = new List<string>();
            do
            {
                resultSegment = await blobClient.ListContainersSegmentedAsync(continuationToken);
                foreach (var blobItem in resultSegment.Results)
                {
                    var url = blobItem.StorageUri.PrimaryUri.ToString();
                    containers.Add(url);
                }

                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

            return containers.ToArray();
        }

        public async Task<string[]> GetBranchesPathsAsync(Project project)
        {
            CloudBlobContainer container = GetContainerReference(project.Id);

            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;

            List<string> blobs = new List<string>();
            do
            {
                resultSegment = await container.ListBlobsSegmentedAsync(string.Empty, false, BlobListingDetails.Metadata, 10, continuationToken, null, null);
                foreach (var blobItem in resultSegment.Results)
                {
                    var url = blobItem.StorageUri.PrimaryUri.ToString();
                    if (url.EndsWith("/"))
                    {
                        blobs.Add(url);
                    }
                }

                continuationToken = resultSegment.ContinuationToken;
            }
            while (continuationToken != null);

            return blobs.ToArray();
        }

        public async Task<string> ReadProjectConfigurationFileAsyc(string path)
        {
            var containerName = PathUtility.GetLastDirectory(path);
            CloudBlobContainer container = GetContainerReference(containerName);

            CloudBlockBlob blockBlob = container.GetBlockBlobReference("configuration.json");

            string text = await blockBlob.DownloadTextAsync();
            return text;
        }

        public async Task<byte[]> ReadAllBytesAsync(Project project, Branch branch, string file)
        {
            CloudBlobContainer container = GetContainerReference(project.Id);

            var path = Path.Combine(branch.Name, file);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);
            using (var memoryStream = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public async Task<string> ReadAllTextAsync(Project project, Branch branch, string file)
        {
            CloudBlobContainer container = GetContainerReference(project.Id);

            var path = Path.Combine(branch.Name, file);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);

            string text = await blockBlob.DownloadTextAsync();
            return text;
        }

        public async Task<ActionConfirmation> CreateProjectAsync(Project project)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.Value.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(project.Id);

            bool exists = await container.ExistsAsync();
            if(exists)
            {
                return ActionConfirmation.CreateError($"Project '{project.Id}' already exists.");
            }

            await container.CreateAsync();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("configuration.json");
            var configurationText = JsonConvert.SerializeObject(project);
            await blockBlob.UploadTextAsync(configurationText);

            return ActionConfirmation.CreateSuccessfull();
        }

        public async Task<ActionConfirmation> UpdateProjectAsync(string projectId, Project project)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.Value.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(projectId);

            bool exists = await container.ExistsAsync();
            if (!exists)
            {
                return ActionConfirmation.CreateError($"Project '{projectId}' not exists.");
            }

            await container.CreateIfNotExistsAsync();

            CloudBlockBlob blockBlob = container.GetBlockBlobReference("configuration.json");
            var configurationText = JsonConvert.SerializeObject(project);
            await blockBlob.UploadTextAsync(configurationText);

            return ActionConfirmation.CreateSuccessfull();
        }

        public async Task<ActionConfirmation> DeleteProjectAsync(string projectId)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.Value.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(projectId);

            bool exists = await container.ExistsAsync();
            if (!exists)
            {
                return ActionConfirmation.CreateError($"Project '{projectId}' not exists.");
            }

            await container.DeleteAsync();
            return ActionConfirmation.CreateSuccessfull();
        }

        public async Task<ActionConfirmation<string>> UploadBranchAsync(Project project, IFormFile file)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.Value.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(project.Id);

            string branchName = string.Empty;
            IList<DocumentIndex> documentDtos = new List<DocumentIndex>();
            using (var zipArchive = new ZipArchive(file.OpenReadStream()))
            {
                var entries = zipArchive.Entries;
                bool mainFolderWasDeleted = false;
                foreach (var entry in entries)
                {
                    bool isDirectory = entry.FullName.EndsWith("/");
                    if (isDirectory)
                    {
                        if (!mainFolderWasDeleted)
                        {
                            CloudBlockBlob mainFolder = container.GetBlockBlobReference(entry.FullName);
                            await mainFolder.DeleteIfExistsAsync();
                            mainFolderWasDeleted = true;
                            branchName = entry.FullName.Trim('/');
                        }

                        continue;
                    }

                    CloudBlockBlob blob = container.GetBlockBlobReference(entry.FullName);
                    using (var stream = entry.Open())
                    {
                        await blob.UploadFromStreamAsync(stream);
                    }

                    if (ShouldAddToSearchIndex(entry))
                    {
                        var content = await blob.DownloadTextAsync();
                        var documentIndex = _searchService.PrepareDocument(entry.FullName, project, branchName, content);
                        documentDtos.Add(documentIndex);
                    }
                }
            }

            await _searchService.ReindexDocumentAsync(project.Id, branchName, documentDtos);
            return ActionConfirmation<string>.CreateSuccessfull(branchName);
        }

        public async Task<ActionConfirmation> DeleteBranchAsync(string projectId, string branchName)
        {
            CloudBlobContainer container = GetContainerReference(projectId);
            CloudBlockBlob branchFolder = container.GetBlockBlobReference(branchName);
            if (branchFolder != null)
            {
                await branchFolder.DeleteIfExistsAsync();
            }

            await _searchService.DeleteDocumemntsAsync(projectId, branchName);
            return ActionConfirmation.CreateSuccessfull();
        }

        private static bool ShouldAddToSearchIndex(ZipArchiveEntry entry)
        {
            return entry.FullName.EndsWith(".md");
        }

        private CloudBlobContainer GetContainerReference(string containerName)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_applicationParameters.Value.AzureStorageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            if (container == null)
            {
                throw new AzureCloudConteinerNotFoundException($"Azure cloud blob container '{containerName}' not exists");
            }

            return container;
        }
    }
}
