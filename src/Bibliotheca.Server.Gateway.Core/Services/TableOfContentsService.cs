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

        public async Task<IList<ChapterItem>> GetTableOfConents(string projectId, string branchName)
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

        private IList<ChapterItem> GetChapterItems(Dictionary<object, object> mkDocsConfiguration)
        {
            var pages = mkDocsConfiguration["pages"];

            var listOfPages = pages as List<object>;
            ChapterItem parentItem = new ChapterItem();
            AddChildItems(parentItem, listOfPages);

            var rootChapterItems = new List<ChapterItem>();
            foreach (var item in parentItem.Children)
            {
                rootChapterItems.Add(item);
            }

            return rootChapterItems;
        }

        private void AddChildItems(ChapterItem parentItem, List<object> pages)
        {
            foreach (var page in pages)
            {
                var subPages = page as Dictionary<object, object>;
                foreach (var key in subPages.Keys)
                {
                    var value = subPages[key];

                    ChapterItem node = new ChapterItem();
                    node.Name = key.ToString();
                    parentItem.Children.Add(node);

                    if (value is string)
                    {
                        node.Url = value.ToString();
                    }
                    else
                    {
                        List<object> nodes = value as List<object>;
                        AddChildItems(node, nodes);
                    }
                }
            }
        }

        public bool TryGetToc(out IList<ChapterItem> toc)
        {
            return _memoryCache.TryGetValue(_tocCacheKey, out toc);
        }

        public void AddToc(IList<ChapterItem> toc)
        {
            _memoryCache.Set(_tocCacheKey, toc,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }
    }
}