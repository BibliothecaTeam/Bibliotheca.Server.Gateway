using Bibliotheca.Server.Gateway.Core.GraphQL.Types;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL.Types;
using Bibliotheca.Server.Gateway.Core.Services;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Bibliotheca.Server.Gateway.Core.Policies;
using System;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Resolvers
{
    public class ProjectsResolver : IProjectsResolver
    {
        private readonly IProjectsService _projectsService;
        private readonly IAuthorizationService _authorizationService;

        public ProjectsResolver(IProjectsService projectsService, IAuthorizationService authorizationService) 
        {
            _projectsService = projectsService;
            _authorizationService = authorizationService;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            graphQLQuery.Field<ProjectsResultsType>(
                "projects",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "groups", Description = "project's groups" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "tags", Description = "project's tags" }
                ),
                resolve: context => { 
                    var groups = context.GetArgument<IList<string>>("groups");
                    var tags = context.GetArgument<IList<string>>("tags");
                    var user = context.UserContext as ClaimsPrincipal;
                    groups = groups.Count == 0 ? null : groups;
                    tags = tags.Count == 0 ? null : tags;
                    return _projectsService.GetProjectsAsync(new ProjectsFilterDto { Groups = groups, Tags = tags }, user.Identity.Name);
                }
            );

            graphQLQuery.Field<ProjectType>(
                "project",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the project" }
                ),
                resolve: context => { 
                    var id = context.GetArgument<string>("id");
                    var project = _projectsService.GetProjectAsync(id);
                    if(project == null) {
                        return null;
                    }

                    var user = context.UserContext as ClaimsPrincipal;
                    var authorization = _authorizationService.AuthorizeAsync(user, project, Operations.Read).GetAwaiter().GetResult();
                    if (!authorization.Succeeded)
                    {
                        return null;
                    }

                    return project;
                }
            );
        }
    }
}