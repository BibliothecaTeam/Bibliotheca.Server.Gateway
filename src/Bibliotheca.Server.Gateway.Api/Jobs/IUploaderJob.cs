using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Bibliotheca.Server.Gateway.Api.Jobs
{
    /// <summary>
    /// Job for uploading new documents.
    /// </summary>
    public interface IUploaderJob
    {
        /// <summary>
        /// Upload new documents to the application.
        /// </summary>
        /// <param name="projectId">Project Id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="body">Documents.</param>
        /// <returns>Returns async task.</returns>
        Task UploadBranchAsync(string projectId, string branchName, byte[] body);

        /// <summary>
        /// Upload new documents to the application.
        /// </summary>
        /// <param name="projectId">Project Id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <returns>Returns async task.</returns>
        Task UploadBranchAsync(string projectId, string branchName);
    }
}