using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;
using Bibliotheca.Server.Depository.Client;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class DocumentsService : IDocumentsService
    {
        private readonly IDocumentsClient _documentsClient;

        public DocumentsService(IDocumentsClient documentsClient)
        {
            _documentsClient = documentsClient;
        }

        public async Task<DocumentDto> GetDocumentAsync(string projectId, string branchName, string fileUri)
        {
            var documentDto = await _documentsClient.Get(projectId, branchName, fileUri);
            return documentDto;
        }

        public async Task CreateDocumentAsync(string projectId, string branchName, DocumentDto document)
        {
            await _documentsClient.Post(projectId, branchName, document);
        }

        public async Task UpdateDocumentAsync(string projectId, string branchName, string fileUri, DocumentDto document)
        {
            await _documentsClient.Put(projectId, branchName, fileUri, document);
        }

        public async Task DeleteDocumentAsync(string projectId, string branchName, string fileUri)
        {
            await _documentsClient.Delete(projectId, branchName, fileUri);
        }
    }
}