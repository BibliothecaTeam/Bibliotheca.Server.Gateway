using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class ProjectAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, ProjectDto>
    {
        private readonly IUsersService _usersService;

        public ProjectAuthorizationHandler(IUsersService usersService)
        {
            _usersService = usersService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            OperationAuthorizationRequirement requirement, 
            ProjectDto project)
        {
            if(requirement == Operations.Read)
            {
                await CheckIfUsertCanRead(context, requirement, project);
            }
            else if(requirement == Operations.Update || requirement == Operations.Delete || requirement == Operations.Create)
            {
                await CheckIfUsertCanModify(context, requirement, project);
            }
        }

        private async Task CheckIfUsertCanModify(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, ProjectDto project)
        {
            if (context.User.Identity.AuthenticationType == "SecureToken")
            {
                if (context.User.Identity.Name == "System")
                {
                    context.Succeed(requirement);
                }
            }
            else if (context.User.Identity.AuthenticationType == "Bearer"
                || context.User.Identity.AuthenticationType == "AuthenticationTypes.Federation"
                || context.User.Identity.AuthenticationType == "UserToken")
            {
                var userId = context.User.Identity.Name.ToLower();
                var user = await _usersService.GetUserAsync(userId);
                if (user != null)
                {
                    if (user.Role == RoleEnumDto.Administrator)
                    {
                        context.Succeed(requirement);
                        return;
                    }

                    if (requirement == Operations.Update && user.Projects.Contains(project.Id))
                    {
                        context.Succeed(requirement);
                    }
                    else if (requirement == Operations.Create && user.Role == RoleEnumDto.Writer)
                    {
                        context.Succeed(requirement);
                    }
                    else if (requirement == Operations.Delete && user.Role == RoleEnumDto.Writer && user.Projects.Contains(project.Id))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }

        private async Task CheckIfUsertCanRead(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement, ProjectDto project)
        {

            if (!project.IsAccessLimited)
            {
                context.Succeed(requirement);
            }
            else
            {
                if (context.User.Identity.AuthenticationType == "SecureToken")
                {
                    if (context.User.Identity.Name == "System")
                    {
                        context.Succeed(requirement);
                        return;
                    }
                }
                else
                {
                    var userId = context.User.Identity.Name.ToLower();
                    var user = await _usersService.GetUserAsync(userId);

                    if (user.Role == RoleEnumDto.Administrator)
                    {
                        context.Succeed(requirement);
                        return;
                    }
                    
                    if (project.ContactPeople.Any(x => x.Email == userId))
                    {
                        context.Succeed(requirement);
                        return;
                    }
                    
                    if (user != null && user.Projects != null && user.Projects.Any(o => o == project.Id))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }
    }
}