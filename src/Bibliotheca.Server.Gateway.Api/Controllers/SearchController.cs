using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Services;
using Bibliotheca.Server.Indexer.Abstractions.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<DocumentSearchResultDto<DocumentIndexDto>> Get([FromQuery] FilterDto filter)
        {
            return await _searchService.SearchAsync(filter);
        }

        
        [HttpGet("projects/{projectId}/branches/{branchName}")]
        public async Task<DocumentSearchResultDto<DocumentIndexDto>> Get([FromQuery] FilterDto filter, string projectId, string branchName)
        {
            return await _searchService.SearchAsync(filter, projectId, branchName);
        }

        [HttpPost("projects/{projectId}/branches/{branchName}")]
        public async Task<IActionResult> Post(string projectId, string branchName, [FromBody] IEnumerable<DocumentIndexDto> documentIndexDtos)
        {
            await _searchService.UploadDocumentsAsync(projectId, branchName, documentIndexDtos);
            return Ok();
        }

        [HttpPost("projects/{projectId}/branches/{branchName}/refresh")]
        public async Task<IActionResult> Post(string projectId, string branchName)
        {
            await _searchService.RefreshIndexAsync(projectId, branchName);
            return Ok();
        }

        [HttpPost("projects/{projectId}/branches/{branchName}/status")]
        public async Task<Core.DataTransferObjects.IndexStatusDto> Get(string projectId, string branchName)
        {
            var result = await _searchService.GetRefreshIndexStatusAsync(projectId, branchName);
            return result;
        }

        [HttpDelete("projects/{projectId}/branches/{branchName}")]
        public async Task<IActionResult> Delete(string projectId, string branchName)
        {
            await _searchService.DeleteDocumentsAsync(projectId, branchName);
            return Ok();
        }
    }
}