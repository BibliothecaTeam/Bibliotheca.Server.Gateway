using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public interface ILogsClient
    {
        Task<LogsDto> Get(string projectId);

        Task<HttpResponseMessage> Put(string projectId, LogsDto logs);
    }
}