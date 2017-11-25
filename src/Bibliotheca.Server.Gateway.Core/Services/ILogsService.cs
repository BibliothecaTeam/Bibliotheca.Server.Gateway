using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface ILogsService
    {
        Task<LogsDto> GetLogsAsync(string projectId);

        Task AppendLogsAsync(string projectId, LogsDto logs);
    }
}