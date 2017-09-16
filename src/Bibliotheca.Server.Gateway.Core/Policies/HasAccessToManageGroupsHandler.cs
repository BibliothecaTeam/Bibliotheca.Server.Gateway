using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Microsoft.AspNetCore.Authorization;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class HasAccessToManageGroupsHandler : AuthorizationHandler<HasAccessToManageGroupsRequirement>
    {
        private readonly IUsersClient _usersClient;

        public HasAccessToManageGroupsHandler(IUsersClient usersClient)
        {
            _usersClient = usersClient;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasAccessToManageGroupsRequirement requirement)
        {
            if(context.User.Identity.AuthenticationType == "SecureToken")
            {
                if (context.User.Identity.Name == "System")
                {
                    context.Succeed(requirement);
                }
            }
            else if(context.User.Identity.AuthenticationType == "Bearer" 
                || context.User.Identity.AuthenticationType == "AuthenticationTypes.Federation"
                || context.User.Identity.AuthenticationType == "UserToken")
            {
                var userId = context.User.Identity.Name.ToLower();

                var user = await _usersClient.Get(userId);
                if (user != null && (user.Role == RoleEnumDto.Administrator))
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}