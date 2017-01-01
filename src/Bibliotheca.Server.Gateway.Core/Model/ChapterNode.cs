using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.Model
{
    public class ChapterNode
    {
        public ChapterNode()
        {
            Children = new List<ChapterNode>();
        }

        public ChapterNode(ChapterNode node) : this()
        {
            Name = node.Name;
            Url = node.Url;
        }

        public string Name { get; set; }

        public string Url { get; set; }

        [JsonIgnore]
        public ChapterNode Parent { get; set; }

        public List<ChapterNode> Children { get; private set; }
    }
}
