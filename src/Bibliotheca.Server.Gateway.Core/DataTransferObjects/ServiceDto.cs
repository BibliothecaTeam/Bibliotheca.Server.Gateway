using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class ServiceDto
    {
        public ServiceDto()
        {
            Instances = new List<InstanceDto>();
        }

        public string Name { get; set; }

        public IList<InstanceDto> Instances { get; set; }
    }
}