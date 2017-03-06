using System;

namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class IndexStatusDto
    {
        public IndexStatusEnum IndexStatus { get; set; }
        
        public string ProjectId { get; set; }

        public string BranchName { get; set; }

        public DateTime StartTime { get; set; }

        public int NumberOfIndexedDocuments { get; set; }

        public int? NumberOfAllDocuments { get; set; }
    }
}