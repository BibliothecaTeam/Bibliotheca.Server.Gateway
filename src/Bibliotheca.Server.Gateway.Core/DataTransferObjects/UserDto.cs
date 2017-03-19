using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class UserDto
    {
        public string Id { get; set; }

        public string Name { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public RoleEnumDto Role { get; set; }

        public IList<UserProjectDto> UserProjects { get; set; }
    }
}