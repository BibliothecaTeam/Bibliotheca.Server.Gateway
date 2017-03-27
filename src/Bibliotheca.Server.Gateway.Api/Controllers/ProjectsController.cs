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
    [Route("api/projects")]
    public class ProjectsController : Controller
    {
        private readonly IProjectsService _projectsService;

        private readonly IAuthorizationService _authorizationService;

        public ProjectsController(IProjectsService projectsService, IAuthorizationService authorizationService)
        {
            _projectsService = projectsService;
            _authorizationService = authorizationService;
        }

        [HttpGet()]
        public async Task<FilteredResutsDto<ProjectDto>> Get([FromQuery] ProjectsFilterDto filter)
        {
            var projects = await _projectsService.GetProjectsAsync(filter);
            return projects;
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> Get(string projectId)
        {
            var project = await _projectsService.GetProjectAsync(projectId);
            if (project == null)
            {
                return NotFound();
            }

            return new ObjectResult(project);
        }

        [HttpPost]
        [Authorize("CanAddProject")]
        public async Task<IActionResult> Post([FromBody] ProjectDto project)
        {
            await _projectsService.CreateProjectAsync(project);
            return Created($"/projects/{project.Id}", project);
        }

        [HttpPut("{projectId}")]
        public async Task<IActionResult> Put(string projectId, [FromBody] ProjectDto project)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if (!isAuthorize)
            {
                return Forbid();
            }

            await _projectsService.UpdateProjectAsync(projectId, project);
            return Ok();
        }

        [HttpDelete("{projectId}")]
        public async Task<IActionResult> Delete(string projectId)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Delete);
            if (!isAuthorize)
            {
                return Forbid();
            }

            await _projectsService.DeleteProjectAsync(projectId);
            return Ok();
        }
    }
}
