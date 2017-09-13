using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Services;
using Bibliotheca.Server.Mvc.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    /// <summary>
    /// Controller which can export markdown dokumentation to different files.
    /// </summary>
    [UserAuthorize]
    [ApiVersion("1.0")]
    [Route("api/projects/{projectId}/branches/{branchName}/export")]
    public class ExportController : Controller
    {
        private readonly IExportService _exportService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="exportService">Export service.</param>
        public ExportController(IExportService exportService)
        {
            _exportService = exportService;
        }

        /// <summary>
        /// Get pdf file.
        /// </summary>
        /// <remarks>
        /// Endpoint returns pdf file which is generated based on markdown files from specific propject and branch.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <returns>Pdf file.</returns>
        [HttpGet("pdf")]
        [ProducesResponseType(200, Type = typeof(FileResult))]
        public async Task<FileResult> Get(string projectId, string branchName)
        {
            var bytes = await _exportService.GeneratePdf(projectId, branchName);
            var fileResult = new FileContentResult(bytes, "application/pdf");
            return fileResult;
        }
    }
}