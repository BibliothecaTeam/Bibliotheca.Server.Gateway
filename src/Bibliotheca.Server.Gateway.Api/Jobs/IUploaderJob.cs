using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bibliotheca.Server.Gateway.Api.Jobs
{
    /// <summary>
    /// Job for uploading new documents.
    /// </summary>
    [Hangfire.Queue("upload")]
    public interface IUploaderJob
    {
        /// <summary>
        /// Upload new documents to the application.
        /// </summary>
        /// <param name="projectId">Project Id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="filePath">File path.</param>
        /// <returns>Returns async task.</returns>
        Task UploadBranchAsync(string projectId, string branchName, string filePath);
    }
}