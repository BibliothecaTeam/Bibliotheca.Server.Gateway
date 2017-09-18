using System.Collections.Generic;
using Newtonsoft.Json;

namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class ProjectDto
    {
        public ProjectDto()
        {
            Tags = new List<string>();
            VisibleBranches = new List<string>();
            EditLinks = new List<EditLinkDto>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string DefaultBranch { get; set; }

        public List<string> VisibleBranches { get; set; }

        public List<string> Tags { get; private set; }

        public string Group { get; set; }

        public string ProjectSite { get; set; }

        public List<ContactPersonDto> ContactPeople { get; set; }

        public List<EditLinkDto> EditLinks { get; set; }

        public string AccessToken { get; set; }

        public bool IsAccessLimited { get; set; }

        [JsonIgnore]
        public List<string> Owners { get; set; }

        public IList<ExtendedBranchDto> Branches { get; set; }
    }
}