using Markdig;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class MarkdownService
    {
        public string ConvertToHtml(string markdown)
        {
            var html = Markdown.ToHtml(markdown);
            return html;
        }
    }
}