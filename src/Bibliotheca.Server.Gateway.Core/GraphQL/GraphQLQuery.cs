using Bibliotheca.Server.Gateway.Core.GraphQL.Types;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL.Types;
using Bibliotheca.Server.Gateway.Core.Services;
using System.Linq;
using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.GraphQL
{
    public class GraphQLQuery : ObjectGraphType
    {
        public GraphQLQuery(
            IProjectsService projectsService, 
            ITagsService tagsService,
            IGroupsService groupsService,
            ITableOfContentsService tableOfContentsService)
        {
            Field<ListGraphType<ProjectType>>(
                "projects",
                arguments: new QueryArguments(
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "groups", Description = "project's groups" },
                    new QueryArgument<ListGraphType<StringGraphType>> { Name = "tags", Description = "project's tags" }
                ),
                resolve: context => { 
                    var groups = context.GetArgument<IList<string>>("groups");
                    var tags = context.GetArgument<IList<string>>("tags");
                    groups = groups.Count == 0 ? null : groups;
                    tags = tags.Count == 0 ? null : tags;
                    return projectsService.GetProjectsAsync(new ProjectsFilterDto { Groups = groups, Tags = tags }, string.Empty).GetAwaiter().GetResult().Results.ToArray();
                }
            );

            Field<ProjectType>(
                "project",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "id", Description = "id of the project" }
                ),
                resolve: context => { 
                    var id = context.GetArgument<string>("id");
                    return projectsService.GetProjectAsync(id);
                }
            );

            Field<ListGraphType<TagType>>(
                "tags",
                resolve: context => { 
                    return tagsService.GetAvailableTagsAsync();
                }
            );

            Field<ListGraphType<GroupType>>(
                "groups",
                resolve: context => { 
                    return groupsService.GetGroupsAsync();
                }
            );

            Field<ListGraphType<ChapterItemType>>(
                "chapters",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "projectId", Description = "id of the project" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "branchName", Description = "name of the branch" }
                ),
                resolve: context => { 
                    var projectId = context.GetArgument<string>("projectId");
                    var branchName = context.GetArgument<string>("branchName");
                    return tableOfContentsService.GetTableOfConents(projectId, branchName);
                }
            );
        }
    }
}