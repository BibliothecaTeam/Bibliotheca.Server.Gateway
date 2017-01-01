using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.Model
{
    public class ProjectsFilter : Filter
    {
        public IList<string> Groups { get; set; }

        public IList<string> Tags { get; set; }
    }
}