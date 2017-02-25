using System.Threading.Tasks;
using Hangfire.Server;

namespace Bibliotheca.Server.Gateway.Api.Jobs
{
    public interface IServiceDiscoveryRegistrationJob
    {
        Task RegisterServiceAsync(PerformContext context);
    }
}