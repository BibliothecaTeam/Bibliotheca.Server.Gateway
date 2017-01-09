using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;

namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class ExtendedBranchDto : BranchDto
    {
        public ExtendedBranchDto(BranchDto branchDto)
        {
            Name = branchDto.Name;
            MkDocsYaml = branchDto.MkDocsYaml;
        }

        public string SiteName { get; set; }

        public string DocsDir { get; set; }
    }
}