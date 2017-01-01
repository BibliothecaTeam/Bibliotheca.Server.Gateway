using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.Model
{
    public class Documentation
    {
        public Documentation()
        {
            Breadcrumbs = new List<ChapterNode>();
        }

        public string PageContent { get; set; }

        [JsonIgnore]
        public Branch Branch { get; set; }

        public ChapterNode CurrentNode { get; set; }

        public List<ChapterNode> Breadcrumbs { get; private set; }
    }
}
