using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Microsoft.Extensions.Caching.Memory;
using YamlDotNet.Serialization;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class TableOfContentsService : ITableOfContentsService
    {
        private readonly IBranchesClient _branchesClient;

        private readonly IMemoryCache _memoryCache;

        public TableOfContentsService(IBranchesClient branchesClient, IMemoryCache memoryCache)
        {
            _branchesClient = branchesClient;
            _memoryCache = memoryCache;
        }

        public async Task<IList<ChapterItemDto>> GetTableOfConents(string projectId, string branchName)
        {
            List<ChapterItemDto> toc = null;
            string cacheKey = GetCacheKey(projectId, branchName);

            if(!_memoryCache.TryGetValue(cacheKey, out toc))
            {
                var branch = await _branchesClient.Get(projectId, branchName);
                if(branch != null)
                {
                    var mkDocsConfiguration = ReadMkDocsConfiguration(branch.MkDocsYaml);
                    toc = GetChapterItems(mkDocsConfiguration);

                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    _memoryCache.Set(cacheKey, toc, cacheEntryOptions);
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

            var listOfPages = pages as List<object>;
            var parentItem = new ChapterItemDto();
            AddChildItems(parentItem, listOfPages, docsDir);

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
                        AddChildItems(node, nodes, docsDir);
                    }
                }
            }
        }

        private string GetCacheKey(string projectId, string branchName)
        {
            return $"TableOfContentsService#{projectId}#{branchName}";
        }
    }
}