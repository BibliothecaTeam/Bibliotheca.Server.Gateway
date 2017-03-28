using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Policies;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/projects/{projectId}/branches/{branchName}/documents")]
    public class DocumentsController : Controller
    {
        private readonly IDocumentsService _documentsService;

        private readonly IMarkdownService _markdownService;

        private readonly IBranchesService _branchService;

        private readonly ISearchService _searchService;

        private readonly IAuthorizationService _authorizationService;

        public DocumentsController(
            IDocumentsService documentsService, 
            IMarkdownService markdownService, 
            IBranchesService branchService,
            ISearchService searchService,
            IAuthorizationService authorizationService)
        {
            _documentsService = documentsService;
            _markdownService = markdownService;
            _branchService = branchService;
            _searchService = searchService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(string projectId, string branchName)
        {
            var documents = await _documentsService.GetDocumentsAsync(projectId, branchName);
            if(documents == null)
            {
                return NotFound();
            }

            return new ObjectResult(documents);
        }

        [HttpGet("{fileUri}")]
        public async Task<IActionResult> Get(string projectId, string branchName, string fileUri)
        {
            var document = await _documentsService.GetDocumentAsync(projectId, branchName, fileUri);
            if(document == null) 
            {
                return NotFound();
            }

            return new ObjectResult(document);
        }

        [HttpGet("content/{fileUri}")]
        public async Task<IActionResult> GetContent(string projectId, string branchName, string fileUri)
        {
            var document = await _documentsService.GetDocumentAsync(projectId, branchName, fileUri);
            if(document == null) 
            {
                return NotFound();
            }

            byte[] content = document.Content;
            string contentType = document.ConentType;

            if(document.ConentType == "text/markdown")
            {
                var markdown = System.Text.Encoding.UTF8.GetString(document.Content);
                var html = _markdownService.ConvertToHtml(markdown);

                content = System.Text.Encoding.UTF8.GetBytes (html);
                contentType = "text/html";
            }

            return new FileContentResult(content, contentType);
        }

        [HttpPost]
        public async Task<IActionResult> Post(string projectId, string branchName, [FromBody] DocumentDto document)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if(!isAuthorize)
            {
                return Forbid();
            }

            await _documentsService.CreateDocumentAsync(projectId, branchName, document);

            document.Content = null;
            return Created($"/projects/{projectId}/branches/{branchName}/documents/{document.Uri}", document);
        }

        [HttpPut("{fileUri}")]
        public async Task<IActionResult> Put(string projectId, string branchName, string fileUri, [FromBody] DocumentDto document)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if(!isAuthorize)
            {
                return Forbid();
            }

            await _documentsService.UpdateDocumentAsync(projectId, branchName, fileUri, document);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put(string projectId, string branchName, IFormFile file)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if(!isAuthorize)
            {
                return Forbid();
            }

            var branches = await _branchService.GetBranchesAsync(projectId);
            if(branches.Any(x => x.Name == branchName))
            {
                await _branchService.DeleteBranchAsync(projectId, branchName);
            }

            await _documentsService.UploadBranchAsync(projectId, branchName, file.OpenReadStream());
            await _searchService.RefreshIndexAsync(projectId, branchName);
            return Ok();
        }

        [HttpPut("filebody")]
        public async Task<IActionResult> Put(string projectId, string branchName)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if(!isAuthorize)
            {
                return Forbid();
            }

            var branches = await _branchService.GetBranchesAsync(projectId);
            if(branches.Any(x => x.Name == branchName))
            {
                await _branchService.DeleteBranchAsync(projectId, branchName);
            }
            
            await _documentsService.UploadBranchAsync(projectId, branchName, Request.Body);
            await _searchService.RefreshIndexAsync(projectId, branchName);
            return Ok();
        }

        [HttpDelete("{fileUri}")]
        public async Task<IActionResult> Delete(string projectId, string branchName, string fileUri)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, Operations.Update);
            if(!isAuthorize)
            {
                return Forbid();
            }

            await _documentsService.DeleteDocumentAsync(projectId, branchName, fileUri);
            return Ok();
        }
    }
}