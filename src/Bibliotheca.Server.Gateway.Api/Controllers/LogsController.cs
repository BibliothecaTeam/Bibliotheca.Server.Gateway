using System.Reflection;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Policies;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api
{
    /// <summary>
    /// Logs controller.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/logs/{projectId}")]
    public class LogsController : Controller
    {
        private readonly IProjectsService _projectsService;

        private readonly IAuthorizationService _authorizationService;

        private readonly ILogsService _logsService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="projectsService">Project service.</param>
        /// <param name="authorizationService">Authorization service.</param>
        /// <param name="logsService">Logs service.</param>
        public LogsController(
            IProjectsService projectsService, 
            IAuthorizationService authorizationService, 
            ILogsService logsService)
        {
            _projectsService = projectsService;
            _authorizationService = authorizationService;
            _logsService = logsService;
        }

        /// <summary>
        /// Get logs for specific project.
        /// </summary>
        /// <remarks>
        /// Endpoint returns logs information for specific prpoject.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <returns>Information abouth health.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(LogsDto))]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Get(string projectId)
        {
            var projectFromStorage = await _projectsService.GetProjectAsync(projectId);
            if (projectFromStorage == null)
            {
                return NotFound();
            }

            var authorization = await _authorizationService.AuthorizeAsync(User, projectFromStorage, Operations.Read);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            var logs = await _logsService.GetLogsAsync(projectId);
            return new ObjectResult(logs);
        }

        /// <summary>
        /// Append logs for specific project.
        /// </summary>
        /// <remarks>
        /// Endpoint appends logs information for specific prpoject.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="logs">Logs for project.</param>
        /// <returns>Information abouth health.</returns>
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> Put(string projectId, [FromBody] LogsDto logs)
        {
            var projectFromStorage = await _projectsService.GetProjectAsync(projectId);
            if (projectFromStorage == null)
            {
                return NotFound();
            }

            var authorization = await _authorizationService.AuthorizeAsync(User, projectFromStorage, Operations.Update);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            await _logsService.AppendLogsAsync(projectId, logs);
            return Ok();
        }
    }
}