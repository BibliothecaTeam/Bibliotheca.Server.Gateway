using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public class UploaderService : IUploaderService
    {
        private readonly IDocumentsService _documentsService;

        private readonly IBranchesService _branchService;

        private readonly ISearchService _searchService;

        private readonly IProjectsService _projectsService;

        private readonly ILogger<UploaderService> _logger;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="documentsService">Documents service.</param>
        /// <param name="branchService">Branch service.</param>
        /// <param name="searchService">Search service.</param>
        /// <param name="projectsService">Projects service.</param>
        public UploaderService(
            IDocumentsService documentsService, 
            IBranchesService branchService,
            ISearchService searchService,
            IProjectsService projectsService,
            ILogger<UploaderService> logger)
        {
            _documentsService = documentsService;
            _branchService = branchService;
            _searchService = searchService;
            _projectsService = projectsService;
            _logger = logger;
        }

        public async Task UploadBranchAsync(string projectId, string branchName, Stream body)
        {
            try
            {
                _logger.LogInformation($"[Uploading] Getting branch information ({projectId}/{branchName}).");
                var branches = await _branchService.GetBranchesAsync(projectId);

                if(branches.Any(x => x.Name == branchName))
                {
                    _logger.LogInformation($"[Uploading] Deleting branch from storage ({projectId}/{branchName}).");
                    await _branchService.DeleteBranchAsync(projectId, branchName);
                    _logger.LogInformation($"[Uploading] Branch deleted ({projectId}/{branchName}).");
                }
                
                _logger.LogInformation($"[Uploading] Upload branch to storage ({projectId}/{branchName}).");
                await _documentsService.UploadBranchAsync(projectId, branchName, body);
                _logger.LogInformation($"[Uploading] Branch uploaded ({projectId}/{branchName}).");

                _logger.LogInformation($"[Uploading] Getting project information ({projectId}/{branchName}).");
                var project = await _projectsService.GetProjectAsync(projectId);

                if(!project.IsAccessLimited) 
                {
                    _logger.LogInformation($"[Uploading] Ordering index refresh ({projectId}/{branchName}).");
                    await _searchService.RefreshIndexAsync(projectId, branchName);
                    _logger.LogInformation($"[Uploading] Index refresh ordered ({projectId}/{branchName}).");
                }
            }
            catch(Exception exception)
            {
                _logger.LogError($"[Uploading] During uploadind exception occurs ({projectId}/{branchName}).");
                _logger.LogError($"[Uploading] Exception: {exception.ToString()}, message: {exception.Message}, stacktrace: {exception.StackTrace}.");
            }
        }
    }
}