using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    /// <summary>
    /// Controller which manage information about documentation groups.
    /// </summary>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/groups")]
    public class GroupsController : Controller
    {
        private readonly IGroupsService _groupsService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="groupsService">Group service.</param>
        public GroupsController(IGroupsService groupsService)
        {
            _groupsService = groupsService;
        }

        /// <summary>
        /// Get all groups.
        /// </summary>
        /// <remarks>
        /// Endpoint returns all groups which are defined in all documentation projects.
        /// </remarks>
        /// <returns>List of all groups.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<string>))]
        public async Task<IList<string>> Get()
        {
            var groups = await _groupsService.GetAvailableGroupsAsync();
            return groups;
        }
    }
}