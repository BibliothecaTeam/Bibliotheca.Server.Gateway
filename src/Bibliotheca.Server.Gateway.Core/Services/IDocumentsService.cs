using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IDocumentsService
    {
        Task<IList<BaseDocumentDto>> GetDocumentsAsync(string projectId, string branchName);

        Task<DocumentDto> GetDocumentAsync(string projectId, string branchName, string fileUri);

        Task CreateDocumentAsync(string projectId, string branchName, DocumentDto document);

        Task UpdateDocumentAsync(string projectId, string branchName, string fileUri, DocumentDto document);

        Task UploadBranchAsync(string projectId, string branchName, Stream body);

        Task DeleteDocumentAsync(string projectId, string branchName, string fileUri);
    }
}