using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public interface IProjectsClient
    {
        Task<IList<ProjectDto>> Get();

        Task<ProjectDto> Get(string projectId);

        Task<HttpResponseMessage> Post(ProjectDto project);

        Task<HttpResponseMessage> Put(string projectId, ProjectDto project);

        Task<HttpResponseMessage> Delete(string projectId);
    }
}