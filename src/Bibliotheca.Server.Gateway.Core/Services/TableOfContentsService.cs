using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Client;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Microsoft.Extensions.Caching.Memory;
using YamlDotNet.Serialization;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class TableOfContentsService : ITableOfContentsService
    {
        private const string _tocCacheKey = "toc";

        private readonly IBranchesClient _branchesClient;

        private readonly IMemoryCache _memoryCache;

        public TableOfContentsService(IBranchesClient branchesClient, IMemoryCache memoryCache)
        {
            _branchesClient = branchesClient;
            _memoryCache = memoryCache;
        }

        public async Task<IList<ChapterItemDto>> GetTableOfConents(string projectId, string branchName)
        {
            var branch = await _branchesClient.Get(projectId, branchName);

            var mkDocsConfiguration = ReadMkDocsConfiguration(branch.MkDocsYaml);
            var rootChapterItems = GetChapterItems(mkDocsConfiguration);

            return rootChapterItems;
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

        private IList<ChapterItemDto> GetChapterItems(Dictionary<object, object> mkDocsConfiguration)
        {
            var pages = mkDocsConfiguration["pages"];

            var listOfPages = pages as List<object>;
            var parentItem = new ChapterItemDto();
            AddChildItems(parentItem, listOfPages);

            var rootChapterItems = new List<ChapterItemDto>();
            foreach (var item in parentItem.Children)
            {
                rootChapterItems.Add(item);
            }

            return rootChapterItems;
        }

        private void AddChildItems(ChapterItemDto parentItem, List<object> pages)
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
                        var url = value.ToString();
                        node.Url = url.Replace("/", ":");
                    }
                    else
                    {
                        List<object> nodes = value as List<object>;
                        AddChildItems(node, nodes);
                    }
                }
            }
        }

        public bool TryGetToc(out IList<ChapterItemDto> toc)
        {
            return _memoryCache.TryGetValue(_tocCacheKey, out toc);
        }

        public void AddToc(IList<ChapterItemDto> toc)
        {
            _memoryCache.Set(_tocCacheKey, toc,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }
    }
}