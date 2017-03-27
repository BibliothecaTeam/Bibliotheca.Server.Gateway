using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Policies;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/projects/{projectId}/branches")]
    public class BranchesController : Controller
    {
        private readonly IBranchesService _branchesService;

        private readonly IAuthorizationService _authorizationService;

        public BranchesController(IBranchesService branchesService, IAuthorizationService authorizationService)
        {
            _branchesService = branchesService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string projectId)
        {
            var branches = await _branchesService.GetBranchesAsync(projectId);
            if (branches == null)
            {
                return NotFound();
            }

            return new ObjectResult(branches);
        }

        [HttpGet("{branchName}")]
        public async Task<IActionResult> Get(string projectId, string branchName)
        {
            var branch = await _branchesService.GetBranchAsync(projectId, branchName);
            if (branch == null)
            {
                return NotFound();
            }

            return new ObjectResult(branch);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string projectId, [FromBody] BranchDto branch)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if (!isAuthorize)
            {
                return Forbid();
            }

            await _branchesService.CreateBranchAsync(projectId, branch);
            return Created($"/projects/{projectId}/branches/{branch.Name}", branch);
        }

        [HttpPut("{branchName}")]
        public async Task<IActionResult> Put(string projectId, string branchName, [FromBody] BranchDto branch)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if (!isAuthorize)
            {
                return Forbid();
            }

            await _branchesService.UpdateBranchAsync(projectId, branchName, branch);
            return Ok();
        }

        [HttpDelete("{branchName}")]
        public async Task<IActionResult> Delete(string projectId, string branchName)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if (!isAuthorize)
            {
                return Forbid();
            }

            await _branchesService.DeleteBranchAsync(projectId, branchName);
            return Ok();
        }
    }
}