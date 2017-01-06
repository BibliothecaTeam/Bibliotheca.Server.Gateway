using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IProjectsService
    {
        Task<IList<ProjectDto>> GetProjectsAsync();
    }
}