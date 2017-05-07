using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public interface IPdfExportClient
    {
        Task<HttpResponseMessage> Post(string markdown);
    }
}