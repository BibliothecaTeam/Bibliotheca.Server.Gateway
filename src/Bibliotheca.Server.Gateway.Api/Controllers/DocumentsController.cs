using System.Collections.Generic;
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
    /// <summary>
    /// Controller which manages documents stored in documentation branch in project.
    /// </summary>
    [ApiVersion("1.0")]
    [Route("api/projects/{projectId}/branches/{branchName}/documents")]
    public class DocumentsController : Controller
    {
        private readonly IDocumentsService _documentsService;

        private readonly IMarkdownService _markdownService;

        private readonly IBranchesService _branchService;

        private readonly ISearchService _searchService;

        private readonly IAuthorizationService _authorizationService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="documentsService">Documents service.</param>
        /// <param name="markdownService">Markdown service.</param>
        /// <param name="branchService">Branch service.</param>
        /// <param name="searchService">Search service.</param>
        /// <param name="authorizationService">Authorization service.</param>
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

        /// <summary>
        /// Get list of documents from branch in project.
        /// </summary>
        /// <remarks>
        /// Endpoint which returns list of documents from specifuc branch in project. 
        /// Each object contains base information about document.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <returns>List of documents.</returns>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IList<BaseDocumentDto>))]
        public async Task<IActionResult> Get(string projectId, string branchName)
        {
            var documents = await _documentsService.GetDocumentsAsync(projectId, branchName);
            if(documents == null)
            {
                return NotFound();
            }

            return new ObjectResult(documents);
        }

        /// <summary>
        /// Get information about specific document.
        /// </summary>
        /// <remarks>
        /// Endpoint returns detailed information about document. Contains also document body as a JSON property.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="fileUri">File uri. As a path separator to file is used ":". For example path "docs/folder/index.md"
        /// should be converted to "docs:folder:index.md".</param>
        /// <returns>Detailed information about document (with document body).</returns>
        [Authorize]
        [HttpGet("{fileUri}")]
        [ProducesResponseType(200, Type = typeof(DocumentDto))]
        public async Task<IActionResult> Get(string projectId, string branchName, string fileUri)
        {
            var document = await _documentsService.GetDocumentAsync(projectId, branchName, fileUri);
            if(document == null) 
            {
                return NotFound();
            }

            return new ObjectResult(document);
        }

        /// <summary>
        /// Get document content.
        /// </summary>
        /// <remarks>
        /// Endpoint returns document content. Markdown files are converted to HTML and returned as a HTML files.
        /// Response returns specific file and proper content type based on requested file.
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="fileUri">File uri. As a path separator to file is used ":". For example path "docs/folder/index.md"
        /// should be converted to "docs:folder:index.md".</param>
        /// <returns>Document content.</returns>
        [Authorize]
        [HttpGet("content/{fileUri}")]
        [ProducesResponseType(200, Type = typeof(FileContentResult))]
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

        /// <summary>
        /// Create new document information.
        /// </summary>
        /// <remarks>
        /// Endpoint which is used to add a new documentation file to branch in project. Documentation files can be send one by one
        /// using this endpoint. However after sending all of the documentation files reindex have to be run manually (if search service is
        /// enabled).
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="document">Document data.</param>
        /// <returns>If created successfully endpoint returns 201 (Created).</returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(201)]
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

        /// <summary>
        /// Update documentation information.
        /// </summary>
        /// <remarks>
        /// Endpoint which is used to update documentation file in branch in project. After updating documentation data reindex have to 
        /// be run manually (if search service is enabled).
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="fileUri">File uri. As a path separator to file is used ":". For example path "docs/folder/index.md"
        /// should be converted to "docs:folder:index.md".</param>
        /// <param name="document">Document data.</param>
        /// <returns>If updated successfully endpoint returns 200 (Ok).</returns>
        [Authorize]
        [HttpPut("{fileUri}")]
        [ProducesResponseType(200)]
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

        /// <summary>
        /// Update all documentation file (form).
        /// </summary>
        /// <remarks>
        /// Endpoint which is used to send all documentation to specific branch. Current documentation files are deleted and
        /// new files are stored instead of them. 
        /// Compressed file should have one root folder which name is the name of the branch. Compressed file should be send as
        /// a part of HTML form in field: "file".
        /// 
        /// After updating documentation files reindex is executed automatically (if you have search service enabled).
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="file">Compressed zip file sent as a form property which have "file" name.</param>
        /// <returns>If updated successfully endpoint returns 200 (Ok).</returns>
        [AllowAnonymous]
        [HttpPut]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Put(string projectId, string branchName, IFormFile file)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, new CanUploadBranchRequirement(HttpContext.Request.Headers));
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

        /// <summary>
        /// Update all documentation file (body).
        /// </summary>
        /// <remarks>
        /// Endpoint which is used to send all documentation to specific branch. Current documentation files are deleted and
        /// new files are stored instead of them. 
        /// Compressed file should have one root folder which name is the name of the branch. Compressed file should be send as
        /// request body.
        /// 
        /// After updating documentation files reindex is executed automatically (if you have search service enabled).
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <returns>If updated successfully endpoint returns 200 (Ok).</returns>
        [AllowAnonymous]
        [HttpPut("filebody")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Put(string projectId, string branchName)
        {
            var isAuthorize = await _authorizationService.AuthorizeAsync(User, new ProjectDto { Id = projectId }, new CanUploadBranchRequirement(HttpContext.Request.Headers));
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

        /// <summary>
        /// Delete documentation file.
        /// </summary>
        /// <remarks>
        /// Endpoint which is used to delete specific documentation file. After deleting documentation file reindex have to 
        /// be run manually (if search service is enabled).
        /// </remarks>
        /// <param name="projectId">Project id.</param>
        /// <param name="branchName">Branch name.</param>
        /// <param name="fileUri">File uri. As a path separator to file is used ":". For example path "docs/folder/index.md"
        /// should be converted to "docs:folder:index.md".</param>
        /// <returns>If deleted successfully endpoint returns 200 (Ok).</returns>
        [Authorize]
        [HttpDelete("{fileUri}")]
        [ProducesResponseType(200)]
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