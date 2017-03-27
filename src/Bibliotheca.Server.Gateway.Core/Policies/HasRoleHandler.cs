using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Microsoft.AspNetCore.Authorization;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class HasRoleHandler : AuthorizationHandler<HasRoleRequirement>
    {
        private readonly IUsersClient _usersClient;

        public HasRoleHandler(IUsersClient usersClient)
        {
            _usersClient = usersClient;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasRoleRequirement requirement)
        {
            var userId = context.User.Identity.Name.ToLower();

            var user = await _usersClient.Get(userId);
            if (user != null && user.Role == requirement.Role)
            {
                context.Succeed(requirement);
            }
        }
    }
}