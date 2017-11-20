using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IUploaderService
    {
        Task UploadBranchAsync(string projectId, string branchName, Stream body);
    }
}