using System.IO;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;
using YamlDotNet.Serialization;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.Gateway.Core.Model;
using Bibliotheca.Server.Gateway.Core.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class FileStorageService : IStorageService
    {
        private readonly IOptions<ApplicationParameters> _applicationParameters;
        private readonly ISearchService _searchService;

        public FileStorageService(IOptions<ApplicationParameters> applicationParameters, ISearchService searchService)
        {
            _applicationParameters = applicationParameters;
            _searchService = searchService;
        }

        public async Task<string[]> GetProjectsPathsAsync()
        {
            return await GetDirectoriesAsync(_applicationParameters.Value.ProjectsUrl);
        }

        public async Task<string[]> GetBranchesPathsAsync(Project project)
        {
            return await GetDirectoriesAsync(project.Url);
        }

        public async Task<string> ReadProjectConfigurationFileAsyc(string path)
        {
            var configurationFilePath = Path.Combine(path, "configuration.json");
            string text = await ReadAllTextAsync(configurationFilePath);
            return text;
        }

        public async Task<byte[]> ReadAllBytesAsync(Project project, Branch branch, string file)
        {
            string path = GetPathToFile(project, branch, file);
            byte[] data = await ReadAllBytesAsync(path);
            return data;
        }

        public async Task<string> ReadAllTextAsync(Project project, Branch branch, string file)
        {
            string path = GetPathToFile(project, branch, file);
            return await ReadAllTextAsync(path);
        }

        public async Task<ActionConfirmation> CreateProjectAsync(Project project)
        {
            var projectDirectory = Path.Combine(_applicationParameters.Value.ProjectsUrl, project.Id);
            if(Directory.Exists(projectDirectory))
            {
                return ActionConfirmation.CreateError($"Project '{project.Id}' already exists.");
            }

            Directory.CreateDirectory(projectDirectory);

            var configurationFilePath = Path.Combine(projectDirectory, "configuration.json");
            var configurationText = JsonConvert.SerializeObject(project);
            await WriteAllTextAsync(configurationFilePath, configurationText);

            return ActionConfirmation.CreateSuccessfull();
        }

        public async Task<ActionConfirmation> UpdateProjectAsync(string projectId, Project project)
        {
            var projectDirectory = Path.Combine(_applicationParameters.Value.ProjectsUrl, projectId);
            if(!Directory.Exists(projectDirectory))
            {
                return ActionConfirmation.CreateError($"Project '{projectId}' not exists.");
            }

            var configurationFilePath = Path.Combine(projectDirectory, "configuration.json");
            if(File.Exists(configurationFilePath))
            {
                File.Delete(configurationFilePath);
            }

            var configurationText = JsonConvert.SerializeObject(project);
            await WriteAllTextAsync(configurationFilePath, configurationText);

            return ActionConfirmation.CreateSuccessfull();
        }

        public Task<ActionConfirmation> DeleteProjectAsync(string projectId)
        {
            var projectDirectory = Path.Combine(_applicationParameters.Value.ProjectsUrl, projectId);
            if (!Directory.Exists(projectDirectory))
            {
                var actionConfirmation = ActionConfirmation.CreateError($"Project '{projectId}' not exists.");
                Task.FromResult(actionConfirmation);
            }

            Directory.Delete(projectDirectory, true);
            return Task.FromResult(ActionConfirmation.CreateSuccessfull());
        }

        public async Task<ActionConfirmation<string>> UploadBranchAsync(Project project, IFormFile file)
        {
            if(!IsYmlFileCorrect(file))
            {
                throw new YmlFileIncorrectException("File 'mkdocs.yml' is incorrect. Verify syntax of yaml file here: http://www.yamllint.com/.");
            }

            IList<DocumentIndex> documentDtos = new List<DocumentIndex>();
            string branchName = string.Empty;
            var projectDirectory = Path.Combine(_applicationParameters.Value.ProjectsUrl, project.Id);
            using (var zipArchive = new ZipArchive(file.OpenReadStream()))
            {
                var entries = zipArchive.Entries;
                bool mainFolderWasDeleted = false;
                foreach (var entry in entries)
                {
                    var path = Path.Combine(projectDirectory, entry.FullName);
                    bool isDirectory = entry.FullName.EndsWith("/");
                    if (isDirectory)
                    {
                        if (!mainFolderWasDeleted)
                        {
                            if (Directory.Exists(path))
                            {
                                Directory.Delete(path, true);
                                branchName = entry.FullName.Trim('/');
                            }

                            mainFolderWasDeleted = true;
                        }

                        Directory.CreateDirectory(path);
                        continue;
                    }

                    using (var stream = entry.Open())
                    {
                        using (var fileStream = File.Create(path))
                        {
                            await stream.CopyToAsync(fileStream);
                        }
                    }

                    if (ShouldAddToSearchIndex(path))
                    {
                        var content = File.ReadAllText(path);
                        var documentIndex = _searchService.PrepareDocument(entry.FullName, project, branchName, content);
                        documentDtos.Add(documentIndex);
                    }
                }
            }

            await _searchService.ReindexDocumentAsync(project.Id, branchName, documentDtos);
            return ActionConfirmation<string>.CreateSuccessfull(branchName);
        }

        private bool IsYmlFileCorrect(IFormFile file)
        {
            using (var zipArchive = new ZipArchive(file.OpenReadStream()))
            {
                var entries = zipArchive.Entries;
                foreach (var entry in entries)
                {
                    bool isDirectory = entry.FullName.EndsWith("/");
                    if (isDirectory)
                    {
                        continue;
                    }

                    if (entry.FullName.EndsWith("mkdocs.yml"))
                    {
                        try
                        {
                            using (var stream = entry.Open())
                            using (TextReader textReader = new StreamReader(stream))
                            {
                                var deserializer = new Deserializer();
                                var banchConfiguration = deserializer.Deserialize(textReader) as Dictionary<object, object>;
                            }

                            return true;
                        }
                        catch(Exception)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public async Task<ActionConfirmation> DeleteBranchAsync(string projectId, string branchName)
        {
            var projectDirectory = Path.Combine(_applicationParameters.Value.ProjectsUrl, projectId);
            var branchFolder = Path.Combine(projectDirectory, branchName);
            if (Directory.Exists(branchFolder))
            {
                Directory.Delete(branchFolder, true);
            }

            await _searchService.DeleteDocumemntsAsync(projectId, branchName);
            return ActionConfirmation.CreateSuccessfull();
        }

        private static bool ShouldAddToSearchIndex(string path)
        {
            return path.EndsWith(".md");
        }

        private string GetPathToFile(Project project, Branch branch, string file)
        {
            string path = Path.Combine(project.Url, branch.Name, file);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            return path;
        }

        private async Task<string[]> GetDirectoriesAsync(string path)
        {
            return await Task.Run(() =>
            {
                return Directory.GetDirectories(path);
            });
        }

        private async Task<string> ReadAllTextAsync(string path)
        {
            return await Task.Run(() =>
            {
                return File.ReadAllText(path);
            });
        }

        private async Task<byte[]> ReadAllBytesAsync(string path)
        {
            return await Task.Run(() =>
            {
                return File.ReadAllBytes(path);
            });
        }

        private async Task WriteAllTextAsync(string path, string content)
        {
            await Task.Run(() =>
            {
                File.WriteAllText(path, content);
            });
        }
    }
}
