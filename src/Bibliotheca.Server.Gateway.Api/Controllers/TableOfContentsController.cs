using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    /// <summary>
    /// Controller which manages table of contents.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/projects/{projectId}/branches/{branchName}/toc")]
    public class TableOfContentsController : Controller
    {
        private readonly ITableOfContentsService _tableOfContentsService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tableOfContentsService">Table of contents service.</param>
        public TableOfContentsController(ITableOfContentsService tableOfContentsService)
        {
            _tableOfContentsService = tableOfContentsService;
        }

        /// <summary>
        /// Get table of contents for specific branch in project.
        /// </summary>
        /// <remarks>
        /// Endpoint is resposible for returning table of contents contained in specific branch in project.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <returns>Table of contents for branch.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<ChapterItemDto>))]
        public async Task<IActionResult> Get(string projectId, string branchName)
        {
            var rootChapterNodes = await _tableOfContentsService.GetTableOfConents(projectId, branchName);
            if(rootChapterNodes == null)
            {
                return NotFound();
            }

            return new ObjectResult(rootChapterNodes);
        }
    }
}