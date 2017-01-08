namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IMarkdownService
    {
        string ConvertToHtml(string markdown);
    }
}