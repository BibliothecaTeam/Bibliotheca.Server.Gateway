using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class IsSecureTokenHandler : AuthorizationHandler<IsSecureTokenRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsSecureTokenRequirement requirement)
        {
            var userId = context.User.Identity.Name;
            var authenticationType = context.User.Identity.AuthenticationType;

            if (authenticationType == "SecureToken" && userId == "System")
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}