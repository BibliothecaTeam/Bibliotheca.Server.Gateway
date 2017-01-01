using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Bibliotheca.Server.Gateway.Core.Services;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/tags")]
    public class TagsController : Controller
    {
        private readonly ITagsService _tagsService;

        public TagsController(ITagsService tagsService)
        {
            _tagsService = tagsService;
        }

        [HttpGet()]
        public async Task<IEnumerable<string>> Get()
        {
            var tags = await _tagsService.GetAvailableTagsAsync();
            return tags;
        }
    }
}
