using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Policies;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    /// <summary>
    /// Controller which manages branches in documentation project.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/projects/{projectId}/branches")]
    public class BranchesController : Controller
    {
        private readonly IBranchesService _branchesService;

        private readonly IAuthorizationService _authorizationService;

        private readonly ISearchService _searchService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="branchesService">Branches service.</param>
        /// <param name="authorizationService">Authorization service.</param>
        /// <param name="searchService">Search service.</param>
        public BranchesController(
            IBranchesService branchesService, 
            IAuthorizationService authorizationService,
            ISearchService searchService)
        {
            _branchesService = branchesService;
            _authorizationService = authorizationService;
            _searchService = searchService;
        }

        /// <summary>
        /// Get all branches for specific project.
        /// </summary>
        /// <remarks>
        /// Endpoint returns all branches for specific project.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <returns>List of branches.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<ExtendedBranchDto>))]
        public async Task<IActionResult> Get(string projectId)
        {
            var branches = await _branchesService.GetBranchesAsync(projectId);
            if (branches == null)
            {
                return NotFound();
            }

            return new ObjectResult(branches);
        }

        /// <summary>
        /// Get information about specific branch in project.
        /// </summary>
        /// <remarks>
        /// Endpoint returns information about one specific project.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <returns>Information about specific branch.</returns>
        [HttpGet("{branchName}")]
        [ProducesResponseType(200, Type = typeof(ExtendedBranchDto))]
        public async Task<IActionResult> Get(string projectId, string branchName)
        {
            var branch = await _branchesService.GetBranchAsync(projectId, branchName);
            if (branch == null)
            {
                return NotFound();
            }

            return new ObjectResult(branch);
        }

        /// <summary>
        /// Create a new branch.
        /// </summary>
        /// <remarks>
        /// Endpoint for creating a new branch in project. Information about branch should be send as a JSON in body request.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branch">Information about branch.</param>
        /// <returns>If created successfully endpoint returns 201 (Created).</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Post(string projectId, [FromBody] BranchDto branch)
        {
            var authorization = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            await _branchesService.CreateBranchAsync(projectId, branch);
            return Created($"/projects/{projectId}/branches/{branch.Name}", branch);
        }

        /// <summary>
        /// Update information about branch.
        /// </summary>
        /// <remarks>
        /// Endpoint for updating information about branch. Information about branch should be send as a JSON in body request.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="branch">Information about branch.</param>
        /// <returns>If updated successfully endpoint returns 200 (Ok).</returns>
        [HttpPut("{branchName}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Put(string projectId, string branchName, [FromBody] BranchDto branch)
        {
            var authorization = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            await _branchesService.UpdateBranchAsync(projectId, branchName, branch);
            return Ok();
        }

        /// <summary>
        /// Delete specific branch.
        /// </summary>
        /// <remarks>
        /// Endpoint for deleting specific branch. Besides branch information endpoint deletes also documentation file 
        /// stored in branch and index from search service.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <returns>If deleted successfully endpoint returns 200 (Ok).</returns>
        [HttpDelete("{branchName}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Delete(string projectId, string branchName)
        {
            var authorization = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if (!authorization.Succeeded)
            {
                return Forbid();
            }

            await _branchesService.DeleteBranchAsync(projectId, branchName);
            await _searchService.DeleteDocumentsAsync(projectId, branchName);
            return Ok();
        }
    }
}