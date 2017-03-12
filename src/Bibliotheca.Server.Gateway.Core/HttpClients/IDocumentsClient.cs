using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public interface IDocumentsClient
    {
        Task<IList<BaseDocumentDto>> Get(string projectId, string branchName);

        Task<DocumentDto> Get(string projectId, string branchName, string fileUri);

        Task<HttpResponseMessage> Post(string projectId, string branchName, DocumentDto document);

        Task<HttpResponseMessage> Put(string projectId, string branchName, string fileUri, DocumentDto document);

        Task<HttpResponseMessage> Delete(string projectId, string branchName, string fileUri);
    }
}