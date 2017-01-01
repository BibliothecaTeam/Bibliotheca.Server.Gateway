using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.Model
{
    public class Project
    {
        private string _name;
        private string _group;

        public Project()
        {
            Branches = new List<Branch>();
            Tags = new List<string>();
            TagsNormmalized = new List<string>();
            VisibleBranches = new List<string>();
        }

        public Project(string url) : this()
        {
            Url = url;
        }

        public string Id { get; set; }

        public string Name 
        { 
            get { return _name; } 
            set 
            { 
                _name = value; 
                NormalizedName = _name.ToUpper();
            } 
        }

        [JsonIgnore]
        public string NormalizedName { get; set; }

        [JsonIgnore]
        public string Url { get; set; }

        public string Description { get; set; }

        public string DefaultBranch { get; set; }

        public List<string> VisibleBranches { get; set; }

        public List<string> Tags { get; private set; }

        [JsonIgnore]
        public List<string> TagsNormmalized { get; private set; }

        public string Group 
        { 
            get { return _group; } 
            set 
            { 
                _group = value;
                GroupNormalized = _group.ToUpper();
            } 
        }

        [JsonIgnore]
        public string GroupNormalized { get; set; }

        [JsonIgnore]
        public Branch DefaultBranchObject { get; set; }

        [JsonIgnore]
        public List<Branch> Branches { get; private set; }
    }
}
