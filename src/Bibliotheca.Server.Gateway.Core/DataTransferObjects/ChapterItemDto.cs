using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class ChapterItemDto
    {
        public ChapterItemDto()
        {
            Children = new List<ChapterItemDto>();
        }

        public ChapterItemDto(ChapterItemDto node) : this()
        {
            Name = node.Name;
            Url = node.Url;
        }

        public string Name { get; set; }

        public string Url { get; set; }

        public IList<ChapterItemDto> Children { get; private set; }
    }
}