using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class HasRoleRequirement : IAuthorizationRequirement
    {
        public RoleEnumDto Role { get; set; }

        public HasRoleRequirement(RoleEnumDto role)
        {
            Role = role;
        }
    }
}