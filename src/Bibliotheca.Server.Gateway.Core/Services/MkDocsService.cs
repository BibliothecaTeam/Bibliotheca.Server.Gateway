using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Model;
using YamlDotNet.Serialization;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class MkDocsService : IMkDocsService
    {
        private readonly IStorageService _storageService;

        public MkDocsService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task GetBranchConfigurationAsync(Project project, Branch branch)
        {
            var mkDocsConfiguration = await ReadMkDocsConfiguration(project, branch);

            if (mkDocsConfiguration.ContainsKey("docs_dir"))
            {
                branch.DocsDir = mkDocsConfiguration["docs_dir"].ToString();
            }

            if (mkDocsConfiguration.ContainsKey("site_name"))
            {
                branch.SiteName = mkDocsConfiguration["site_name"].ToString();
            }

            InitializeChapters(branch, mkDocsConfiguration);
        }

        private void InitializeChapters(Branch branch, Dictionary<object, object> mkDocsConfiguration)
        {
            var pages = mkDocsConfiguration["pages"];

            var listOfPages = pages as List<object>;
            ChapterNode parentNode = new ChapterNode();
            AddChildNodes(parentNode, listOfPages);

            foreach (var item in parentNode.Children)
            {
                branch.RootChapterNodes.Add(item);
            }
        }

        private void AddChildNodes(ChapterNode parentNode, List<object> pages)
        {
            foreach (var page in pages)
            {
                var subPages = page as Dictionary<object, object>;
                foreach (var key in subPages.Keys)
                {
                    var value = subPages[key];

                    ChapterNode node = new ChapterNode();
                    node.Name = key.ToString();
                    node.Parent = parentNode;
                    parentNode.Children.Add(node);

                    if (value is string)
                    {
                        node.Url = value.ToString();
                    }
                    else
                    {
                        List<object> nodes = value as List<object>;
                        AddChildNodes(node, nodes);
                    }
                }
            }
        }

        private async Task<Dictionary<object, object>> ReadMkDocsConfiguration(Project project, Branch branch)
        {
            var yamlFileContent = await _storageService.ReadAllTextAsync(project, branch, "mkdocs.yml");
            var reader = new StringReader(yamlFileContent);
            var deserializer = new Deserializer();

            var banchConfiguration = deserializer.Deserialize(reader) as Dictionary<object, object>;
            return banchConfiguration;
        }
    }
}
