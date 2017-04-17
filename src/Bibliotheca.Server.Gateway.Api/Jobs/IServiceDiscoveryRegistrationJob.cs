using System.Threading.Tasks;
using Hangfire.Server;

namespace Bibliotheca.Server.Gateway.Api.Jobs
{
    /// <summary>
    /// Job for register in service discovery.
    /// </summary>
    public interface IServiceDiscoveryRegistrationJob
    {
        /// <summary>
        /// Register service in service discovery application.
        /// </summary>
        /// <param name="context">Context.</param>
        /// <returns>Returns async task.</returns>
        Task RegisterServiceAsync(PerformContext context);
    }
}