using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/projects")]
    public class ProjectsController : Controller
    {
        private readonly IProjectsService _projectsService;

        public ProjectsController(IProjectsService projectsService)
        {
            _projectsService = projectsService;
        }

        [HttpGet()]
        public async Task<IList<ProjectDto>> Get()
        {
            var projects = await _projectsService.GetProjectsAsync();
            return projects;
        }

        [HttpGet("{projectId}")]
        public async Task<ProjectDto> Get(string projectId)
        {
            var project = await _projectsService.GetProjectAsync(projectId);
            return project;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProjectDto project)
        {
            await _projectsService.CreateProjectAsync(project);
            return Created($"/projects/{project.Id}", project);
        }

        [HttpPut("{projectId}")]
        public async Task<IActionResult> Put(string projectId, [FromBody] ProjectDto project)
        {
            await _projectsService.UpdateProjectAsync(projectId, project);
            return Ok();
        }

        [HttpDelete("{projectId}")]
        public async Task<IActionResult> Delete(string projectId)
        {
            await _projectsService.DeleteProjectAsync(projectId);
            return Ok();
        }
    }
}
