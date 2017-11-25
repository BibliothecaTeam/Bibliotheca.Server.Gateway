using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.HttpClients;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class LogService : ILogsService
    {
        private readonly ILogsClient _logsClient;


        public LogService(ILogsClient logsClient)
        {
            _logsClient = logsClient;
        }

        public async Task<LogsDto> GetLogsAsync(string projectId)
        {
            return await _logsClient.Get(projectId);
        }

        public async Task AppendLogsAsync(string projectId, LogsDto logs)
        {
            await _logsClient.Put(projectId, logs);
        }
    }
}