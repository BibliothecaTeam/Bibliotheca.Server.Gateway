using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Bibliotheca.Server.Gateway.Core.MimeTypes;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class ExportService : IExportService
    {
        private readonly ITableOfContentsService _tableOfContentsService;

        private readonly IDocumentsService _documentsService;

        private readonly IProjectsService _projectsService;

        private readonly IPdfExportClient _pdfExportClient;

        private class ImageUrl
        {
            public string RelativePath { get; set; }
            public string AbsolutePath { get; set; }
            public string ImageTag { get; set; }
        }

        public ExportService(
            ITableOfContentsService tableOfContentsService,
            IDocumentsService documentsService,
            IProjectsService projectsService,
            IPdfExportClient pdfExportClient)
        {
            _tableOfContentsService = tableOfContentsService;
            _documentsService = documentsService;
            _projectsService = projectsService;
            _pdfExportClient = pdfExportClient;
        }

        public async Task<byte[]> GeneratePdf(string projectId, string branchName)
        {
            var project = await _projectsService.GetProjectAsync(projectId);
            if (project == null)
            {
                throw new ProjectNotFoundException($"Project '{projectId}' not exists.");
            }

            var chapters = await _tableOfContentsService.GetTableOfConents(projectId, branchName);

            var markdownBuilder = new StringBuilder();
            AddTitlePage(project, branchName, markdownBuilder);
            AddPageBreak(markdownBuilder);
            AddTableOfContents(chapters, markdownBuilder);
            AddPageBreak(markdownBuilder);

            var imagesList = new List<ImageUrl>();
            await AddDocumentsContent(projectId, branchName, chapters, markdownBuilder, imagesList);

            var markdown = markdownBuilder.ToString();
            await EmbedImagesToMarkdown(project.Id, branchName, imagesList, markdown);

            var response = await _pdfExportClient.Post(markdown);
            if (response.IsSuccessStatusCode)
            {
                var responseBytes = await response.Content.ReadAsByteArrayAsync();
                return responseBytes;
            }

            var responseString = await response.Content.ReadAsStringAsync();
            throw new PdfExportException($"Exception during generating pdf. Status code: {response.StatusCode}. Message: {responseString}.");
        }

        private async Task EmbedImagesToMarkdown(string projectId, string branchName, List<ImageUrl> imagesList, string markdown)
        {
            foreach (var image in imagesList)
            {
                var imageFile = await _documentsService.GetDocumentAsync(projectId, branchName, image.AbsolutePath);
                if (imageFile != null)
                {
                    var base64Image = System.Convert.ToBase64String(imageFile.Content);
                    var mimeType = MimeTypeMap.GetMimeType(Path.GetExtension(image.AbsolutePath));

                    markdown = markdown.Replace(image.ImageTag, $"<img src=\"data:{mimeType};base64,{base64Image}\" />");
                }
            }
        }

        private void AddTableOfContents(IList<ChapterItemDto> chapters, StringBuilder markdownBuilder)
        {
            markdownBuilder.AppendLine("<ul>");
            foreach (var item in chapters)
            {
                markdownBuilder.Append("<li>");
                
                if (!string.IsNullOrWhiteSpace(item.Name))
                {
                    markdownBuilder.Append($"<span>{item.Name}</span>");
                }

                if(item.Children != null && item.Children.Count > 0)
                {
                    AddTableOfContents(item.Children, markdownBuilder);
                }

                markdownBuilder.Append("</li>");
            }
            markdownBuilder.AppendLine("</ul>");
        }

        private void AddTitlePage(ProjectDto project, string branchName, StringBuilder markdownBuilder)
        {
            var dateString = DateTime.Now.ToString("dd MMMM yyyy");
            var htmlVersion = $"<div style=\"text-align: right;\"><div>version: {branchName}</div><div>date: {dateString}</div></div>";
            var htmlTitle=  $"<div style=\"margin-top: 200px\"><center><h1>{project.Name}</h1></center></div>";

            var htmlDescriptiom = $"<div><div style=\"text-align: center; width: 350px;margin-top: 50px; margin-left: auto; margin-right: auto;\">{project.Description}</div></div>";

            markdownBuilder.AppendLine(htmlVersion);
            markdownBuilder.AppendLine(htmlTitle);
            markdownBuilder.AppendLine(htmlDescriptiom);
        }

        private void AddPageBreak(StringBuilder markdownBuilder)
        {
            markdownBuilder.AppendLine("<p style=\"page-break-after:always;\"></p>");
        }

        private async Task AddDocumentsContent(
            string projectId, 
            string branchName, 
            IList<DataTransferObjects.ChapterItemDto> chapters, 
            StringBuilder markdownBuilder,
            List<ImageUrl> imagesList)
        {
            foreach (var item in chapters)
            {
                if (!string.IsNullOrWhiteSpace(item.Url))
                {
                    var document = await _documentsService.GetDocumentAsync(projectId, branchName, item.Url);
                    var markdown = Encoding.UTF8.GetString(document.Content);

                    imagesList.AddRange(GetImages(item.Url, markdown));

                    markdownBuilder.Append(markdown);
                    markdownBuilder.AppendLine().AppendLine();
                }

                if (item.Children != null && item.Children.Count > 0)
                {
                    await AddDocumentsContent(projectId, branchName, item.Children, markdownBuilder, imagesList);
                }
            }
        }

        private string GetImagePath(string url, string imageUrl)
        {
            var prefixFolder = Path.GetDirectoryName(url.Replace(":", "/"));

            var path = prefixFolder + "/" + imageUrl;
            path = path.Replace("\\\\", "/");
            path = path.Replace("\\", "/");
            path = path.Replace("////", "/");
            path = path.Replace("//", "/");
            path = path.Replace(":", "/");

            var pathParts = path.Split('/');

            var dotsNumber = 0;
            var pathParsed = new List<string>();
            for (var i = pathParts.Length - 1; i >= 0; --i) {
                if (pathParts[i] == "..") {
                    dotsNumber++;
                }
                else {
                    if (dotsNumber == 0) {
                        pathParsed.Add(pathParts[i]);
                    }
                    else {
                        dotsNumber--;
                    }
                }
            }

            pathParsed.Reverse();
            path = string.Join(":", pathParsed);
            return path;
        }

        private List<ImageUrl> GetImages(string markdownUrl, string markdown)
        {
            var imagesList = new List<ImageUrl>();
            var matches = Regex.Matches(markdown, "!\\[[^\\]]+\\]\\([^)]+\\)");
            foreach (Match match in matches)
            {
                var startIndex = match.Value.IndexOf("(");
                var path = match.Value.Substring(startIndex + 1).TrimEnd(')');
                if(!path.StartsWith("http://") && !path.StartsWith("https://"))
                {
                    imagesList.Add(new ImageUrl 
                    { 
                        ImageTag = match.Value, 
                        RelativePath = path,
                        AbsolutePath = GetImagePath(markdownUrl, path)
                    });
                }
            }

            return imagesList;
        }
    }
}