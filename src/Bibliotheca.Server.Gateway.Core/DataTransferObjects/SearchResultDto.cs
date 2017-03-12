namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class SearchResultDto<TDocument>
    {
        public SearchResultDto()
        {
            Highlights = new HitHighlightsDto();
        }

        public TDocument Document { get; set; }

        public HitHighlightsDto Highlights { get; private set; }

        public double Score { get; set; }
    }
}