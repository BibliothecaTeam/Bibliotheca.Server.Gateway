using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Bibliotheca.Server.Gateway.Core.Services;
using Bibliotheca.Server.Gateway.Core.Model;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/projects/{projectId}/branches/{branchName}/toc")]
    public class TableOfContentsController : Controller
    {
        private readonly IProjectsService _projectsService;

        public TableOfContentsController(IProjectsService projectService)
        {
            _projectsService = projectService;
        }

        [HttpGet()]
        public async Task<IEnumerable<ChapterNode>> Get(string projectId, string branchName)
        {
            var branch = await _projectsService.GetBranchAsync(projectId, branchName);
            return branch.RootChapterNodes;
        }
    }
}
