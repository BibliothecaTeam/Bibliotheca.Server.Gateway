using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public interface INightcrawlerClient
    {
        Task<HttpResponseMessage> Post(string projectId, string branchName);

        Task<IndexStatusDto> Get(string projectId, string branchName);
    }
}