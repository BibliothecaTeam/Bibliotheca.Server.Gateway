using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public interface IProjectsClient
    {
        IHttpContextHeaders CustomHeaders { get; }
        
        Task<IList<ProjectDto>> Get();

        Task<ProjectDto> Get(string projectId);

        Task<HttpResponseMessage> Post(ProjectDto project);

        Task<HttpResponseMessage> Put(string projectId, ProjectDto project);

        Task<HttpResponseMessage> Delete(string projectId);
    }
}