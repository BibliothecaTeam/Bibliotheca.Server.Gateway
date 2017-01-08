using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IDocumentsService
    {
        Task<DocumentDto> GetDocumentAsync(string projectId, string branchName, string fileUri);

        Task CreateDocumentAsync(string projectId, string branchName, DocumentDto document);

        Task UpdateDocumentAsync(string projectId, string branchName, string fileUri, DocumentDto document);

        Task DeleteDocumentAsync(string projectId, string branchName, string fileUri);
    }
}