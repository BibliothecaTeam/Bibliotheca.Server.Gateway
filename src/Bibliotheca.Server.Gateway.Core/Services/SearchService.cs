using HtmlAgilityPack;
using Markdig;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.Gateway.Core.Model;
using Bibliotheca.Server.Gateway.Core.Extensions;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class SearchService : ISearchService
    {
        private const string IndexName = "documents";
        private readonly IOptions<ApplicationParameters> _applicationParameters;

        public SearchService(IOptions<ApplicationParameters> applicationParameters)
        {
            _applicationParameters = applicationParameters;
        }

        public async Task<DocumentSearchResult<DocumentIndex>> SearchAsync(Filter filter)
        {
            return await SearchAsync(filter, null, null);
        }

        public async Task<DocumentSearchResult<DocumentIndex>> SearchAsync(Filter filter, string projectId, string branchName)
        {
            SearchIndexClient indexClient = GetSearchIndexClient();

            int? skip = null;
            int? top = null;
            if(filter.Limit > 0)
            {
                skip = filter.Page * filter.Limit;
                top = filter.Limit;
            }

            string branchFilter = null;
            if(!string.IsNullOrWhiteSpace(projectId) && !string.IsNullOrWhiteSpace(branchName))
            {
                branchFilter = $"projectId eq '{projectId}' and branchName eq '{branchName}'";
            }
            
            return await SearchDocumentsAsync(indexClient, filter.Query, skip, top, branchFilter);
        }

        public async Task DeleteIndexIfExistsAsync()
        {
            SearchServiceClient serviceClient = GetSearchServiceClient();
            await DeleteIndexIfExistsAsync(serviceClient);
        }

        public async Task CreateOrUpdateIndexAsync()
        {
            SearchServiceClient serviceClient = GetSearchServiceClient();
            await CreateOrUpdateIndexAsync(serviceClient);
        }

        public async Task DeleteDocumemntsAsync(string projectId, string branchName)
        {
            if (!IsSearchIndexEnabled())
            {
                return;
            }

            SearchIndexClient indexClient = GetSearchIndexClient();
            await DeleteDocumemntsAsync(indexClient, projectId, branchName);
        }

        public async Task ReindexDocumentAsync(string projectId, string branchName, IEnumerable<DocumentIndex> documentDtos)
        {
            if(!IsSearchIndexEnabled())
            {
                return;
            }

            SearchIndexClient indexClient = GetSearchIndexClient();

            await DeleteDocumemntsAsync(indexClient, projectId, branchName);
            await UploadDocumentsAsync(indexClient, documentDtos);
        }

        public DocumentIndex PrepareDocument(string url, Project project, string branchName, string content)
        {
            string indexed = ClearContent(content);
            string title = ClearTitle(url);

            var document = new DocumentIndex
            {
                Id = url.ToAlphanumeric(),
                Url = url,
                ProjectId = project.Id,
                BranchName = branchName,
                Tags = project.Tags.ToArray(),
                Title = title,
                Content = indexed,
                ProjectName = project.Name
            };

            return document;
        }

        private async Task DeleteDocumemntsAsync(SearchIndexClient indexClient, string projectId, string branchName)
        {
            var documents = await SearchDocumentsAsync(indexClient, "*", null, null, $"projectId eq '{projectId}' and branchName eq '{branchName}'");
            if (documents != null && documents.Results != null && documents.Results.Count > 0)
            {
                var keyValues = documents.Results.Select(x => x.Document.Id);

                try
                {
                    var batch = IndexBatch.Delete("id", keyValues);
                    await indexClient.Documents.IndexAsync(batch);
                }
                catch (IndexBatchException e)
                {
                    Console.WriteLine("Failed to index some of the documents: {0}",
                        string.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
                }
            }
        }

        private static string ClearTitle(string url)
        {
            var title = Path.GetFileNameWithoutExtension(url);
            title = Regex.Replace(title, "[^a-zA-Z0-9- _]", string.Empty);
            title = title.Replace("-", " ");
            title = title.Replace("_", " ");
            title = title.UppercaseFirst();
            return title;
        }

        private static string ClearContent(string content)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            var markdownHtml = Markdown.ToHtml(content, pipeline);

            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(markdownHtml);
            var indexed = htmlDocument.DocumentNode.InnerText;

            return indexed;
        }

        private async Task DeleteIndexIfExistsAsync(SearchServiceClient serviceClient)
        {
            if (await serviceClient.Indexes.ExistsAsync(IndexName))
            {
                await serviceClient.Indexes.DeleteAsync(IndexName);
            }
        }

        private async Task CreateOrUpdateIndexAsync(SearchServiceClient serviceClient)
        {
            var definition = new Index()
            {
                Name = IndexName,
                Fields = new[]
                {
                    new Field("id", DataType.String) { IsKey = true },
                    new Field("url", DataType.String) { IsRetrievable = true },
                    new Field("title", DataType.String) { IsRetrievable = true },
                    new Field("projectId", DataType.String) { IsRetrievable = true, IsFilterable = true },
                    new Field("projectName", DataType.String) { IsRetrievable = true, IsFilterable = true },
                    new Field("branchName", DataType.String) { IsRetrievable = true, IsFilterable = true },
                    new Field("tags", DataType.Collection(DataType.String)) { IsRetrievable = true, IsFilterable = true, IsFacetable = true },
                    new Field("content", DataType.String) { IsSearchable = true }
                }
            };

            await serviceClient.Indexes.CreateOrUpdateAsync(definition);
        }

        private SearchIndexClient GetSearchIndexClient()
        {
            SearchServiceClient serviceClient = GetSearchServiceClient();
            SearchIndexClient indexClient = serviceClient.Indexes.GetClient(IndexName);
            return indexClient;
        }

        private SearchServiceClient GetSearchServiceClient()
        {
            string searchServiceName = _applicationParameters.Value.AzureSearchServiceName;
            string apiKey = _applicationParameters.Value.AzureSearchApiKey;

            SearchServiceClient serviceClient = new SearchServiceClient(searchServiceName, new SearchCredentials(apiKey));
            return serviceClient;
        }

        private bool IsSearchIndexEnabled()
        {
            if(!string.IsNullOrWhiteSpace(_applicationParameters.Value.AzureSearchServiceName) &&
                !string.IsNullOrWhiteSpace(_applicationParameters.Value.AzureSearchApiKey))
            {
                return true;
            }

            return false;
        }

        private async Task<DocumentSearchResult<DocumentIndex>> SearchDocumentsAsync(SearchIndexClient indexClient, string query, int? skip = null, int? top = null, string filter = null)
        {
            var searchParameters = new SearchParameters();
            searchParameters.HighlightFields = new[] { "content" };
            searchParameters.QueryType = QueryType.Full;
            searchParameters.IncludeTotalResultCount = true;
            searchParameters.Skip = skip;
            searchParameters.Top = top;
            searchParameters.SearchMode = SearchMode.All;

            if (!string.IsNullOrEmpty(filter))
            {
                searchParameters.Filter = filter;
            }

            DocumentSearchResult<DocumentIndex> response = await indexClient.Documents.SearchAsync<DocumentIndex>(query, searchParameters);
            return response;
        }

        private async Task UploadDocumentsAsync(SearchIndexClient indexClient, IEnumerable<DocumentIndex> documents)
        {
            try
            {
                var batch = IndexBatch.MergeOrUpload(documents);
                await indexClient.Documents.IndexAsync(batch);
            }
            catch (IndexBatchException e)
            {
                Console.WriteLine("Failed to index some of the documents: {0}",
                    string.Join(", ", e.IndexingResults.Where(r => !r.Succeeded).Select(r => r.Key)));
            }
        }
    }
}
