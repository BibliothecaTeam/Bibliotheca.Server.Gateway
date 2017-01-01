using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Bibliotheca.Server.Gateway.Core.Services;
using Bibliotheca.Server.Gateway.Core.Model;
using Bibliotheca.Server.Gateway.Core.MimeTypes;
using Bibliotheca.Server.Gateway.Core.Utilities;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/projects/{projectId}/branches/{branchName}/documentation")]
    public class DocumentationController : Controller
    {
        private readonly IDocumentationService _documentationService;
        private readonly IProjectsService _projectsService;

        public DocumentationController(IDocumentationService documentationService, IProjectsService projectsService)
        {
            _documentationService = documentationService;
            _projectsService = projectsService;
        }

        [Authorize]
        [HttpGet("{file}")]
        public async Task<Documentation> Get(string projectId, string branchName, string file)
        {
            var encodedFile = WebUtility.UrlDecode(file);
            var documentation = await _documentationService.GetDocumentationAsync(projectId, branchName, encodedFile);
            return documentation;
        }

        [HttpGet("binary/{baseFile}/{file}")]
        public async Task<IActionResult> Get(string projectId, string branchName, string baseFile, string file)
        {
            string extension = Path.GetExtension(file);
            string mimeType = MimeTypeMap.GetMimeType(extension);

            var path = await GetPathToFile(projectId, branchName, baseFile, file);
            byte[] fileContent = await _documentationService.GetBinaryFileAsync(projectId, branchName, path);

            return new FileContentResult(fileContent, mimeType);
        }

        private async Task<string> GetPathToFile(string projectId, string branchName, string baseFile, string file)
        {
            var branch = await _projectsService.GetBranchAsync(projectId, branchName);

            var firstPart = WebUtility.UrlDecode(baseFile);
            firstPart = Path.GetDirectoryName(firstPart);

            var secondPart = WebUtility.UrlDecode(file);
            var path = Path.Combine(firstPart, secondPart);

            path = PathUtility.GetFullPath(branch.DocsDir, path);
            return path;
        }
    }
}
