using Microsoft.AspNetCore.Authorization;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class HasAccessToCreateProjectRequirement : IAuthorizationRequirement
    {
        public HasAccessToCreateProjectRequirement()
        {
        }
    }
}