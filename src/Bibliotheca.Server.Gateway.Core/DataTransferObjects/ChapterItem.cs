using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class ChapterItem
    {
        public ChapterItem()
        {
            Children = new List<ChapterItem>();
        }

        public ChapterItem(ChapterItem node) : this()
        {
            Name = node.Name;
            Url = node.Url;
        }

        public string Name { get; set; }

        public string Url { get; set; }

        public IList<ChapterItem> Children { get; private set; }
    }
}