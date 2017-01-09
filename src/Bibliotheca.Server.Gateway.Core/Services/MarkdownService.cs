using Markdig;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class MarkdownService : IMarkdownService
    {
        public string ConvertToHtml(string markdown)
        {
            var html = Markdown.ToHtml(markdown);
            return html;
        }
    }
}