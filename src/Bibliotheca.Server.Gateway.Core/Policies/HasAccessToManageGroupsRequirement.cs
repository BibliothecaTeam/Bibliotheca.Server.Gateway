using Microsoft.AspNetCore.Authorization;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class HasAccessToManageGroupsRequirement : IAuthorizationRequirement
    {
        public HasAccessToManageGroupsRequirement()
        {
        }
    }
}