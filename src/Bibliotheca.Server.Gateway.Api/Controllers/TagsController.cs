using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Services;
using Bibliotheca.Server.Mvc.Middleware.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    /// <summary>
    /// Controller which manage information about documentation tags.
    /// </summary>
    [UserAuthorize]
    [ApiVersion("1.0")]
    [Route("api/tags")]
    public class TagsController : Controller
    {
        private readonly ITagsService _tagsService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tagsService">Tags service.</param>
        public TagsController(ITagsService tagsService)
        {
            _tagsService = tagsService;
        }

        /// <summary>
        /// Get all tags.
        /// </summary>
        /// <remarks>
        /// Endpoint returns all tags which are defined in all documentation projects.
        /// </remarks>
        /// <returns>List of all tags.</returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<string>))]
        public async Task<IList<string>> Get()
        {
            var tags = await _tagsService.GetAvailableTagsAsync();
            return tags;
        }
    }
}