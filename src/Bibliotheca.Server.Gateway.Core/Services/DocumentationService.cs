using Markdig;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bibliotheca.Server.Gateway.Core.Model;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class DocumentationService : IDocumentationService
    {
        private readonly IProjectsService _projectService;
        private readonly IMemoryCache _memoryCache;
        private readonly IStorageService _storageService;

        public DocumentationService(IProjectsService projectService, IMemoryCache memoryCache, IStorageService storageService)
        {
            _projectService = projectService;
            _memoryCache = memoryCache;
            _storageService = storageService;
        }

        public async Task<Documentation> GetDocumentationAsync(string projectId, string branchName, string file)
        {
            var documentation = new Documentation();
            documentation.PageContent = await GetMarkdownFileAsync(projectId, branchName, file);
            
            var branch = await _projectService.GetBranchAsync(projectId, branchName);
 
            documentation.CurrentNode = GetCurrentNode(branch, file);
            CreateBreadcrumbs(documentation);

            return documentation;
        }

        public async Task<string> GetMarkdownFileAsync(string projectId, string branchName, string file)
        {
            string markdownHtmlCacheKey = $"markdown-html-{projectId}-{branchName}-{file}";
            string markdownHtml;
            if (!_memoryCache.TryGetValue(markdownHtmlCacheKey, out markdownHtml))
            {
                markdownHtml = await GenerateMarkdownHtmlAsync(projectId, branchName, file);
                AddToCache(markdownHtmlCacheKey, markdownHtml);
            }

            return markdownHtml;
        }

        public async Task<byte[]> GetBinaryFileAsync(string project, string branchName, string file)
        {
            Branch branchObject = await _projectService.GetBranchAsync(project, branchName);
            byte[] fileContent = await _storageService.ReadAllBytesAsync(branchObject.Project, branchObject, file);
            return fileContent;
        }

        private void CreateBreadcrumbs(Documentation documentation)
        {
            var node = documentation.CurrentNode;
            while (node != null)
            {
                var breadcrumbNode = new ChapterNode(node);
                documentation.Breadcrumbs.Add(breadcrumbNode);

                node = node.Parent;
            }
            
            documentation.Breadcrumbs.Reverse();
        }

        private ChapterNode GetCurrentNode(Branch branch, string page)
        {
            var charsToRemove = branch.DocsDir.Length + 1;
            var openedUrl = page.Substring(charsToRemove, page.Length - charsToRemove);
            return FindChapterNode(branch.RootChapterNodes, openedUrl);
        }

        private ChapterNode FindChapterNode(List<ChapterNode> chapterNodes, string openedUrl)
        {
            foreach (var node in chapterNodes)
            {
                if (node.Url == openedUrl)
                {
                    return node;
                }

                if (node.Children != null && node.Children.Count > 0)
                {
                    var founded = FindChapterNode(node.Children, openedUrl);
                    if (founded != null)
                    {
                        return founded;
                    }
                }
            }

            return null;
        }

        private void AddToCache(string markdownHtmlCachekey, string markdownHtml)
        {
            _memoryCache.Set(markdownHtmlCachekey, markdownHtml,
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));
        }

        private async Task<string> GenerateMarkdownHtmlAsync(string project, string branchName, string file)
        {
            Branch branchObject = await _projectService.GetBranchAsync(project, branchName);
            string fileContent = await _storageService.ReadAllTextAsync(branchObject.Project, branchObject, file);

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var markdownHtml = Markdown.ToHtml(fileContent, pipeline);

            return markdownHtml;
        }
    }
}
