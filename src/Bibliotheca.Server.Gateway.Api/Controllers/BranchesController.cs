using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
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

        public BranchesController(IBranchesService branchesService)
        {
            _branchesService = branchesService;
        }

        [HttpGet]
        public async Task<IList<ExtendedBranchDto>> Get(string projectId)
        {
            var branches = await _branchesService.GetBranchesAsync(projectId);
            return branches;
        }

        [HttpGet("{branchName}")]
        public async Task<ExtendedBranchDto> Get(string projectId, string branchName)
        {
            var branch = await _branchesService.GetBranchAsync(projectId, branchName);
            return branch;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string projectId, [FromBody] BranchDto branch)
        {
            await _branchesService.CreateBranchAsync(projectId, branch);
            return Created($"/projects/{projectId}/branches/{branch.Name}", branch);
        }

        [HttpPut("{branchName}")]
        public async Task<IActionResult> Put(string projectId, string branchName, [FromBody] BranchDto branch)
        {
            await _branchesService.UpdateBranchAsync(projectId, branchName, branch);
            return Ok();
        }

        [HttpDelete("{branchName}")]
        public async Task<IActionResult> Delete(string projectId, string branchName)
        {
            await _branchesService.DeleteBranchAsync(projectId, branchName);
            return Ok();
        }
    }
}