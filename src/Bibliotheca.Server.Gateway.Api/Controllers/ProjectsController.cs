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

        private readonly IUsersService _usersService;

        public ProjectsController(
            IProjectsService projectsService, 
            IAuthorizationService authorizationService, 
            IUsersService usersService)
        {
            _projectsService = projectsService;
            _authorizationService = authorizationService;
            _usersService = usersService;
        }

        [HttpGet()]
        public async Task<FilteredResutsDto<ProjectDto>> Get([FromQuery] ProjectsFilterDto filter)
        {
            var projects = await _projectsService.GetProjectsAsync(filter, User.Identity.Name);
            return projects;
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> Get(string projectId)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Read);
            if (!isAuthorize)
            {
                return Forbid();
            }

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
            await _usersService.AddProjectToUserAsync(User.Identity.Name, project.Id);
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

        [HttpGet("{projectId}/accessToken")]
        public async Task<IActionResult> GetProjectAccessToken(string projectId)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if (!isAuthorize)
            {
                return Forbid();
            }

            AccessTokenDto accessToken = await _projectsService.GetProjectAccessTokenAsync(projectId);
            return new ObjectResult(accessToken);
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
