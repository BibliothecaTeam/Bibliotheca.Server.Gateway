using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Bibliotheca.Server.Gateway.Core.HttpClients;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class ExportService : IExportService
    {
        private readonly ITableOfContentsService _tableOfContentsService;

        private readonly IDocumentsService _documentsService;

        private readonly IPdfExportClient _pdfExportClient;

        public ExportService(
            ITableOfContentsService tableOfContentsService,
            IDocumentsService documentsService,
            IPdfExportClient pdfExportClient)
        {
            _tableOfContentsService = tableOfContentsService;
            _documentsService = documentsService;
            _pdfExportClient = pdfExportClient;
        }

        public async Task<byte[]> GeneratePdf(string projectId, string branchName)
        {
            var chapters = await _tableOfContentsService.GetTableOfConents(projectId, branchName);

            var markdownBuilder = new StringBuilder();
            await BuildMarkdownString(projectId, branchName, chapters, markdownBuilder);

            var markdown = markdownBuilder.ToString();
            var response = await _pdfExportClient.Post(markdown);

            if(response.IsSuccessStatusCode) 
            {
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                return responseBytes;
            }

            var responseString = await response.Content.ReadAsStringAsync();
            throw new PdfExportException($"Exception during generating pdf. Status code: {response.StatusCode}. Message: {responseString}.");
        }

        private async Task BuildMarkdownString(
            string projectId, 
            string branchName, 
            IList<DataTransferObjects.ChapterItemDto> chapters, 
            StringBuilder markdownBuilder)
        {
            foreach (var item in chapters)
            {
                if (!string.IsNullOrWhiteSpace(item.Url))
                {
                    var fileUri = item.Url.Replace("/", ":");
                    var document = await _documentsService.GetDocumentAsync(projectId, branchName, fileUri);
                    var markdown = Encoding.UTF8.GetString(document.Content);

                    markdownBuilder.Append(markdown);
                    markdownBuilder.AppendLine().AppendLine();
                }

                if(item.Children != null && item.Children.Count > 0)
                {
                    await BuildMarkdownString(projectId, branchName, item.Children, markdownBuilder);
                }
            }
        }
    }
}