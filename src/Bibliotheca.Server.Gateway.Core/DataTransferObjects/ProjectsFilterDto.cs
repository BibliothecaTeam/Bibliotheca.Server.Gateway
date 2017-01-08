using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class ProjectsFilterDto : FilterDto
    {
        public IList<string> Groups { get; set; }

        public IList<string> Tags { get; set; }
    }
}