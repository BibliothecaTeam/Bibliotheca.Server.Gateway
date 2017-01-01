using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Model;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IMkDocsService
    {
        Task GetBranchConfigurationAsync(Project project, Branch branch);
    }
}
