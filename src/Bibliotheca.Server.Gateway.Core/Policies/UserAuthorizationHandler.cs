using System;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class UserAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, UserDto>
    {
        private readonly IUsersService _usersService;

        public UserAuthorizationHandler(IUsersService usersService)
        {
            _usersService = usersService;
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
                || context.User.Identity.AuthenticationType == "AuthenticationTypes.Federation"
                || context.User.Identity.AuthenticationType == "UserToken")
            {
                var userId = context.User.Identity.Name.ToLower();

                var user = await _usersService.GetUserAsync(userId);
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