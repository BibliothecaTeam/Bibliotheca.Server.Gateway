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
            graphQLQuery.Field<ResponseGraphType<ProjectsResultsType, FilteredResutsDto<ProjectDto>>>(
                "projects",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "groups", Description = "Limit projects by project group." },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "tags", Description = "Limit projects by project tags." },
                    new QueryArgument<StringGraphType> { Name = "query", Description = "Search projects which contains specific query." },
                    new QueryArgument<IntGraphType> { Name = "page", Description = "Number of page to show." },
                    new QueryArgument<IntGraphType> { Name = "limit", Description = "Number of rows to show on page." }
                ),
                resolve: context => { 
                    var groups = context.GetArgument<IList<string>>("groups");
                    var tags = context.GetArgument<IList<string>>("tags");
                    var query = context.GetArgument<string>("query");

                    int page = 0;
                    if(context.Arguments["page"] != null)
                    {
                        page = context.GetArgument<int>("page", 10);
                    }

                    int limit = 0;
                    if(context.Arguments["limit"] != null)
                    {
                        limit = context.GetArgument<int>("limit", 10);
                    }

                    var user = context.UserContext as ClaimsPrincipal;
                    groups = groups.Count == 0 ? null : groups;
                    tags = tags.Count == 0 ? null : tags;
                    
                    var projects = _projectsService.GetProjectsAsync(
                        new ProjectsFilterDto { 
                            Groups = groups, 
                            Tags = tags, 
                            Query = query, 
                            Page = page, 
                            Limit = limit 
                        }, 
                        user.Identity.Name
                    ).GetAwaiter().GetResult();

                    return new ResponseDto<FilteredResutsDto<ProjectDto>>(projects);
                }
            );

            graphQLQuery.Field<ResponseGraphType<ProjectType, ProjectDto>>(
                "project",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the project" }
                ),
                resolve: context => { 
                    var id = context.GetArgument<string>("id");
                    var project = _projectsService.GetProjectAsync(id).GetAwaiter().GetResult();
                    if(project == null) {
                        return new ResponseDto<ProjectDto>("NotFound", "Project not found.");
                    }

                    var user = context.UserContext as ClaimsPrincipal;
                    var authorization = _authorizationService.AuthorizeAsync(user, project, Operations.Read).GetAwaiter().GetResult();
                    if (!authorization.Succeeded)
                    {
                        return new ResponseDto<ProjectDto>("AccessDenied", "Access denied.");
                    }

                    return new ResponseDto<ProjectDto>(project);
                }
            );
        }
    }
}