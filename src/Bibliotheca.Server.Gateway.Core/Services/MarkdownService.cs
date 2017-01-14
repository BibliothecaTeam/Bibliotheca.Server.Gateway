using Markdig;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class MarkdownService : IMarkdownService
    {
        public string ConvertToHtml(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseBootstrap().Build();
            var html = Markdown.ToHtml(markdown, pipeline);
            return html;
        }
    }
}