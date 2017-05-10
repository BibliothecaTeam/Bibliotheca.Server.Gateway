using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Microsoft.Extensions.Caching.Memory;
using YamlDotNet.Serialization;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class TableOfContentsService : ITableOfContentsService
    {
        private readonly IBranchesClient _branchesClient;

        private readonly IProjectsService _projectsService;

        private readonly ICacheService _cacheService;

        public TableOfContentsService(IBranchesClient branchesClient, IProjectsService projectsService, ICacheService cacheService)
        {
            _branchesClient = branchesClient;
            _projectsService = projectsService;
            _cacheService = cacheService;
        }

        public async Task<IList<ChapterItemDto>> GetTableOfConents(string projectId, string branchName)
        {
            if(branchName == null || branchName == "undefined")
            {
                branchName = await GetDefaultBranch(projectId);
            }

            IList<ChapterItemDto> toc = null;
            if(!_cacheService.TryGetTableOfContents(projectId, branchName, out toc))
            {
                var branch = await _branchesClient.Get(projectId, branchName);
                if(branch != null)
                {
                    var mkDocsConfiguration = ReadMkDocsConfiguration(branch.MkDocsYaml);
                    toc = GetChapterItems(mkDocsConfiguration);

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    _cacheService.AddTableOfContentsToCache(projectId, branchName, toc);
                }
            }

            return toc;
        }

        private Dictionary<object, object> ReadMkDocsConfiguration(string yamlFileContent)
        {
            using (var reader = new StringReader(yamlFileContent))
            {
                var deserializer = new Deserializer();

                var banchConfiguration = deserializer.Deserialize(reader) as Dictionary<object, object>;
                return banchConfiguration;
            }
        }

        private List<ChapterItemDto> GetChapterItems(Dictionary<object, object> mkDocsConfiguration)
        {
            var pages = mkDocsConfiguration["pages"];

            var docsDir = "docs";
            if(mkDocsConfiguration.ContainsKey("docs_dir"))
            {
                var mkDocsdir = mkDocsConfiguration["docs_dir"].ToString();
                if(!string.IsNullOrWhiteSpace(mkDocsdir))
                {
                    docsDir = mkDocsdir;
                }
            }

            var parentItem = new ChapterItemDto();
            var listOfPages = pages as List<object>;
            if(listOfPages != null)
            {
                AddChildItems(parentItem, listOfPages, docsDir);
            }

            var rootChapterItems = new List<ChapterItemDto>();
            foreach (var item in parentItem.Children)
            {
                rootChapterItems.Add(item);
            }

            return rootChapterItems;
        }

        private void AddChildItems(ChapterItemDto parentItem, List<object> pages, string docsDir)
        {
            foreach (var page in pages)
            {
                var subPages = page as Dictionary<object, object>;
                foreach (var key in subPages.Keys)
                {
                    var value = subPages[key];

                    var node = new ChapterItemDto();
                    node.Name = key.ToString();
                    parentItem.Children.Add(node);

                    if (value is string)
                    {
                        var url = $"{docsDir}:{value}";
                        node.Url = url.Replace("/", ":"); 
                    }
                    else
                    {
                        List<object> nodes = value as List<object>;
                        if(nodes != null)
                        {
                            AddChildItems(node, nodes, docsDir);
                        }
                    }
                }
            }
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
    }
}