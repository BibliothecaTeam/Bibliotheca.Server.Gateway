using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Bibliotheca.Server.Gateway.Core.Services;
using Bibliotheca.Server.Gateway.Core.Model;
using Bibliotheca.Server.Gateway.Core.Exceptions;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/projects/{projectId}/branches/")]
    public class BranchesController : Controller
    {
        private readonly IProjectsService _projectService;

        public BranchesController(IProjectsService projectService)
        {
            _projectService = projectService;
        }

        [Route("/api/branches")]
        [HttpGet()]
        public async Task<IEnumerable<Branch>> Get()
        {
            var branches = await _projectService.GetBranchesAsync();
            return branches;
        }

        [HttpGet()]
        public async Task<IEnumerable<Branch>> Get(string projectId)
        {
            var project = await _projectService.GetProjectAsync(projectId);
            if (project == null)
            {
                throw new ProjectNotFoundException($"Project '{projectId}' not found.");
            }

            var branches = await _projectService.GetBranchesAsync(projectId);
            return branches;
        }

        [HttpGet("{branchName}")]
        public async Task<Branch> Get(string projectId, string branchName)
        {
            var branch = await _projectService.GetBranchAsync(projectId, branchName);
            return branch;
        }

        [HttpPost()]
        public async Task<ActionResult> Post(string projectId, IFormFile file)
        {
            var project = await _projectService.GetProjectAsync(projectId);
            if (project == null)
            {
                throw new ProjectNotFoundException($"Project '{projectId}' not found.");
            }

            var result = await _projectService.UploadBranchAsync(project, file);
            return Created($"/branches/{projectId}/{result.ObjectData.Name}", result.ObjectData);
        }

        [HttpPut()]
        public async Task<ActionResult> Put(string projectId, IFormFile file)
        {
            var project = await _projectService.GetProjectAsync(projectId);
            if(project == null)
            {
                throw new ProjectNotFoundException($"Project '{projectId}' not found.");
            }

            var result = await _projectService.UploadBranchAsync(project, file);
            return Ok();
        }

        [HttpDelete("{branchName}")]
        public async Task<ActionResult> Delete(string projectId, string branchName)
        {
            var result = await _projectService.DeleteBranchAsync(projectId, branchName);
            return Ok();
        }
    }
}
