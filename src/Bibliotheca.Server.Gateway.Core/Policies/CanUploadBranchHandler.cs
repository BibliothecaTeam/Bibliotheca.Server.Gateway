using System;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class CanUploadBranchHandler : AuthorizationHandler<CanUploadBranchRequirement, ProjectDto>
    {
        private readonly IUsersService _usersService;

        private readonly IProjectsService _projectsService;

        public CanUploadBranchHandler(IUsersService usersService, IProjectsService projectsService)
        {
            _usersService = usersService;
            _projectsService = projectsService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            CanUploadBranchRequirement requirement, 
            ProjectDto project)
        {
            if(context.User.Identity.AuthenticationType == "SecureToken")
            {
                if (context.User.Identity.Name == "System")
                {
                    context.Succeed(requirement);
                }
            }
            else if(RequestHasProjectAccessToken(requirement))
            {
                var authorization = requirement.Headers["Authorization"].ToString();
                var authorizationParts = authorization.Split(' ');
                if(authorizationParts.Length == 2)
                {
                    var projectToken = authorizationParts[1];
                    var existingToken = await _projectsService.GetProjectAccessTokenAsync(project.Id);
                    if(existingToken != null && !string.IsNullOrWhiteSpace(existingToken.AccessToken))
                    {
                        if(string.Equals(projectToken, existingToken.AccessToken, StringComparison.OrdinalIgnoreCase))
                        {
                            context.Succeed(requirement);
                        }
                    }
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
                    else if(user.Projects.Contains(project.Id))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }

        private bool RequestHasProjectAccessToken(CanUploadBranchRequirement requirement)
        {
            if(!requirement.Headers.ContainsKey("Authorization"))
            {
                return false;
            }

            var authorizationHeader = requirement.Headers["Authorization"].ToString();
            if(authorizationHeader.StartsWith("ProjectToken"))
            {
                return true;
            }

            return false;
        }
    }
}