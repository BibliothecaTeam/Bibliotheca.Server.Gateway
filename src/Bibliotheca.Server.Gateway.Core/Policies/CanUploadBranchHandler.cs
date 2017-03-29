using System;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.HttpClients;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Bibliotheca.Server.Gateway.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class CanUploadBranchHandler : AuthorizationHandler<CanUploadBranchRequirement, ProjectDto>
    {
        private readonly IUsersService _usersService;

        private readonly IProjectsClient _projectsClient;

        private readonly ApplicationParameters _applicationParameters;

        public CanUploadBranchHandler(IUsersService usersService, IProjectsClient projectsClient, IOptions<ApplicationParameters> applicationParameters)
        {
            _usersService = usersService;
            _projectsClient = projectsClient;
            _applicationParameters = applicationParameters.Value;
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

                    _projectsClient.CustomHeaders.Remove("Authorization");
                    _projectsClient.CustomHeaders.Add("Authorization", $"SecureToken {_applicationParameters.SecureToken}");
                    
                    var existingProject = await _projectsClient.Get(project.Id);
                    if(existingProject != null && !string.IsNullOrWhiteSpace(existingProject.AccessToken))
                    {
                        if(string.Equals(projectToken, existingProject.AccessToken, StringComparison.OrdinalIgnoreCase))
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