using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/projects/{projectId}/branches/{branchName}/toc")]
    public class TableOfContentsController : Controller
    {
        private readonly ITableOfContentsService _tableOfContentsService;

        public TableOfContentsController(ITableOfContentsService tableOfContentsService)
        {
            _tableOfContentsService = tableOfContentsService;
        }

        [HttpGet()]
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