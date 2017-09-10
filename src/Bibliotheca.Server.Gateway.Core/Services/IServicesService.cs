using System.Collections.Generic;
using System.Threading.Tasks;
using Neutrino.Entities.Model;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IServicesService
    {
        Task<IList<Service>> GetServicesAsync();

        Task<Service> GetServiceInstanceAsync(string serviceType);
    }
}