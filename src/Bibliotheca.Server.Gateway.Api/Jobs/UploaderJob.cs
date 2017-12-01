using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Api.Jobs
{
    /// <summary>
    /// Job for uploading new documents.
    /// </summary>
    public class UploaderJob : IUploaderJob
    {
        private readonly IDocumentsService _documentsService;

        private readonly IBranchesService _branchService;

        private readonly ISearchService _searchService;

        private readonly IProjectsService _projectsService;
        private readonly ILogsService _logsService;

        private readonly ILogger<UploaderJob> _logger;

        private readonly IHttpContextHeaders _httpContextHeaders;

        private readonly ApplicationParameters _applicationParameters;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="documentsService">Documents service.</param>
        /// <param name="branchService">Branch service.</param>
        /// <param name="searchService">Search service.</param>
        /// <param name="projectsService">Projects service.</param>
        /// <param name="logsService">Logs service.</param>
        /// <param name="logger">Logger service.</param>
        /// <param name="httpContextHeaders">Http context headers.</param>
        /// <param name="applicationParameters">Application parameters.</param>
        public UploaderJob(
            IDocumentsService documentsService, 
            IBranchesService branchService,
            ISearchService searchService,
            IProjectsService projectsService,
            ILogsService logsService,
            ILogger<UploaderJob> logger,
            IHttpContextHeaders httpContextHeaders,
            IOptions<ApplicationParameters> applicationParameters)
        {
            _documentsService = documentsService;
            _branchService = branchService;
            _searchService = searchService;
            _projectsService = projectsService;
            _logsService = logsService;
            _logger = logger;
            _httpContextHeaders = httpContextHeaders;
            _applicationParameters = applicationParameters.Value;
        }

        /// <summary>
        /// Upload new documents to the application.
        /// </summary>
        /// <param name="projectId">Project Id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="filePath">File path.</param>
        /// <returns>Returns async task.</returns>
        public async Task UploadBranchAsync(string projectId, string branchName, string filePath)
        {
            StringBuilder logs = new StringBuilder();

            try
            {
                LogInformation(logs, $"Starting uploading project ({projectId}/{branchName}). Temporary file name: {filePath}.");

                _httpContextHeaders.Headers = new Dictionary<string, StringValues>();
                _httpContextHeaders.Headers.Add("Authorization", $"SecureToken {_applicationParameters.SecureToken}");

                LogInformation(logs, $"Getting branches information ({projectId}/{branchName}).");
                var branches = await _branchService.GetBranchesAsync(projectId);
                LogInformation(logs, $"Branches information retrieved ({projectId}/{branchName}).");

                if(branches.Any(x => x.Name == branchName))
                {
                    LogInformation(logs, $"Deleting branch from storage ({projectId}/{branchName}).");
                    await _branchService.DeleteBranchAsync(projectId, branchName);
                    LogInformation(logs, $"Branch was deleted ({projectId}/{branchName}).");
                }
                
                LogInformation(logs, $"Uploading branch to storage ({projectId}/{branchName}).");
                LogInformation(logs, $"Reading temporary file ({projectId}/{branchName}) from path: '{filePath}'.");
                using(FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    LogInformation(logs, $"File '{filePath}' exists and was readed.");
                    await _documentsService.UploadBranchAsync(projectId, branchName, fileStream, logs);
                }
                LogInformation(logs, $"Branch uploaded ({projectId}/{branchName}).");

                LogInformation(logs, $"Getting project information ({projectId}/{branchName}).");
                var project = await _projectsService.GetProjectAsync(projectId);

                if(!project.IsAccessLimited) 
                {
                    LogInformation(logs, $"Ordering index refresh ({projectId}/{branchName}).");
                    await _searchService.RefreshIndexAsync(projectId, branchName);
                    LogInformation(logs, $"Index refresh ordered ({projectId}/{branchName}).");
                }

                LogInformation(logs, $"Uploading project ({projectId}/{branchName}) finished successfully.");
            }
            catch(Exception exception)
            {
                LogError(logs, $"During uploadind exception occurs ({projectId}/{branchName}).");
                LogError(logs, $"Exception: {exception.ToString()}.");
                LogError(logs, $"Message: {exception.Message}.");
                LogError(logs, $"Stack trace: {exception.StackTrace}.");

                if(exception.InnerException != null)
                {
                    LogError(logs, $"Inner exception: {exception.InnerException.ToString()}.");
                    LogError(logs, $"Inner exception message: {exception.InnerException.Message}.");
                    LogError(logs, $"Inner exception stack trace: {exception.InnerException.StackTrace}.");
                }
                
                LogError(logs, $"Uploading project ({projectId}/{branchName}) failed.");
            }
            finally
            {
                LogInformation(logs, $"Deleting temporary file: '{filePath}'.");
                File.Delete(filePath);
                LogInformation(logs, $"Temporary file '{filePath}' was deleted.");
            }

            var logsDto = new LogsDto { Message = logs.ToString() };
            await _logsService.AppendLogsAsync(projectId, logsDto);
        }

        private void LogInformation(StringBuilder stringBuilder, string message)
        {
            stringBuilder.AppendLine($"[{DateTime.UtcNow}] {message}");
            _logger.LogInformation($"[Uploading] {message}.");
        }

        private void LogError(StringBuilder stringBuilder, string message)
        {
            stringBuilder.AppendLine($"[{DateTime.UtcNow}] {message}");
            _logger.LogError($"[Uploading] {message}.");
        }
    }
}