using System;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class UserAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, UserDto>
    {
        private readonly IUsersClient _usersClient;

        public UserAuthorizationHandler(IUsersClient usersClient)
        {
            _usersClient = usersClient;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement, 
            UserDto userDto)
        {
            if(context.User.Identity.AuthenticationType == "SecureToken")
            {
                if (context.User.Identity.Name == "System")
                {
                    context.Succeed(requirement);
                }
            }
            else if(context.User.Identity.AuthenticationType == "Bearer" 
                || context.User.Identity.AuthenticationType == "UserToken")
            {
                var userId = context.User.Identity.Name.ToLower();

                var user = await _usersClient.Get(userId);
                if (user != null)
                {
                    if(user.Role == RoleEnumDto.Administrator)
                    {
                        context.Succeed(requirement);
                    }
                    else if(string.Equals(userId, userDto.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }
}