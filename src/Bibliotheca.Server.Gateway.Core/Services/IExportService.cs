using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IExportService
    {
        Task<byte[]> GeneratePdf(string projectId, string branchName);
        Task<string> GenerateMarkdown(string projectId, string branchName);
    }
}