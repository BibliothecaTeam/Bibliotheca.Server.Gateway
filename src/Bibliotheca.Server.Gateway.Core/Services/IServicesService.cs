using System.Collections.Generic;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.ServiceDiscovery.ServiceClient.Model;

namespace Bibliotheca.Server.Gateway.Core.Services
{
    public interface IServicesService
    {
        Task<IList<ServiceDto>> GetServicesAsync();

        Task<InstanceDto> GetServiceInstanceAsync(string tag);
    }
}