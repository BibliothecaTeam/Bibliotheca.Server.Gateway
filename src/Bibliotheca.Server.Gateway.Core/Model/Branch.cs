using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.Model
{
    public class Branch
    {
        public Branch()
        {
            DocsDir = "docs";
            RootChapterNodes = new List<ChapterNode>();
        }

        [JsonIgnore]
        public Project Project { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public string Url { get; set; }

        public string ProjectId { get; set; }

        public string DocsDir { get; set; }

        public string SiteName { get; set; }

        [JsonIgnore]
        public List<ChapterNode> RootChapterNodes { get; private set; }
    }
}
