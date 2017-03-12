using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class DocumentSearchResultDto<TDocument>
    {
        public DocumentSearchResultDto()
        {
            Results = new List<SearchResultDto<TDocument>>();
        }

        public int NumberOfResults { get; set; }

        public long ElapsedMilliseconds { get; set; }

        public IList<SearchResultDto<TDocument>> Results { get; private set; }
    }
}