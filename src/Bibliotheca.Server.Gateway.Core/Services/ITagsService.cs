using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface ITagsService
    {
        Task<IList<string>> GetAvailableTagsAsync();
    }
}