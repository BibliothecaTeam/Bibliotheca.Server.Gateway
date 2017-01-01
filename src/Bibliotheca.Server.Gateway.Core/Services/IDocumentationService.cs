using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Model;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IDocumentationService
    {
        Task<Documentation> GetDocumentationAsync(string projectId, string branchName, string file);

        Task<string> GetMarkdownFileAsync(string projectId, string branchName, string file);

        Task<byte[]> GetBinaryFileAsync(string projectId, string branchName, string file);
    }
}
