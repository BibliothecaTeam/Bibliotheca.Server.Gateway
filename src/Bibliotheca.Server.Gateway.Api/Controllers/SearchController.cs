using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Search.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Bibliotheca.Server.Gateway.Core.Services;
using Bibliotheca.Server.Gateway.Core.Model;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/search")]
    public class SearchController : Controller
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [HttpGet()]
        public async Task<DocumentSearchResult<DocumentIndex>> Get([FromQuery] Filter filter)
        {
            var searchResult = await _searchService.SearchAsync(filter);
            return searchResult;
        }

        [HttpGet("projects/{projectId}/branches/{branchName}")]
        public async Task<DocumentSearchResult<DocumentIndex>> Get([FromQuery] Filter filter, string projectId, string branchName)
        {
            var searchResult = await _searchService.SearchAsync(filter, projectId, branchName);
            return searchResult;
        }
    }
}
