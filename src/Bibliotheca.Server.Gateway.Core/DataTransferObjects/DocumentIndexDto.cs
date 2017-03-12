namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class DocumentIndexDto
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string FileUri { get; set; }

        public string Title { get; set; }

        public string ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string BranchName { get; set; }

        public string Content { get; set; }
        
        public string[] Tags { get; set; }       
    }
}