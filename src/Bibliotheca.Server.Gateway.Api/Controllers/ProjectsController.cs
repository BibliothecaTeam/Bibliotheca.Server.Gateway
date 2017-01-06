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
        private readonly IProjectsService _projectService;

        public ProjectsController(IProjectsService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet()]
        public async Task<IList<ProjectDto>> Get()
        {
            var projects = await _projectService.GetProjectsAsync();
            return projects;
        }
    }
}
