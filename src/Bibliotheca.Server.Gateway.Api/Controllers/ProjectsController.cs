using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Model;
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
        private readonly IProjectsService _projectService;

        public ProjectsController(IProjectsService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet()]
        public async Task<FilteredResuts<Project>> Get([FromQuery] ProjectsFilter filter)
        {
            var projects = await _projectService.GetProjectsAsync(filter);
            return projects;
        }

        [HttpGet("{projectId}")]
        public async Task<Project> Get(string projectId)
        {
            var project = await _projectService.GetProjectAsync(projectId);
            return project;
        }

        [HttpPost()]
        public async Task<ActionResult> Post([FromBody] Project project)
        {
            var result = await _projectService.CreateProjectAsync(project);
            return Created($"/projects/{result.ObjectData.Id}", result.ObjectData);
        }

        [HttpPut("{projectId}")]
        public async Task<ActionResult> Put(string projectId, [FromBody] Project project)
        {
            var result = await _projectService.UpdateProjectAsync(projectId, project);
            return Ok();
        }

        [HttpDelete("{projectId}")]
        public async Task<ActionResult> Delete(string projectId)
        {
            var result = await _projectService.DeleteProjectAsync(projectId);
            return Ok();
        }
    }
}
