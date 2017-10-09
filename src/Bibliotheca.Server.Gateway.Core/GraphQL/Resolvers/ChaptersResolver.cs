using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.GraphQL.Types;
using Bibliotheca.Server.Gateway.Core.Services;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Resolvers
{
    public class ChaptersResolver : IChaptersResolver
    {
        private readonly ITableOfContentsService _tableOfContentsService;

        public ChaptersResolver(ITableOfContentsService tableOfContentsService)
        {
            _tableOfContentsService = tableOfContentsService;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            graphQLQuery.Field<ResponseListGraphType<ChapterItemType, ChapterItemDto>>(
                "chapters",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "projectId", Description = "id of the project" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "branchName", Description = "name of the branch" }
                ),
                resolve: context => { 
                    var projectId = context.GetArgument<string>("projectId");
                    var branchName = context.GetArgument<string>("branchName");
                    var chapters =  _tableOfContentsService.GetTableOfConents(projectId, branchName).GetAwaiter().GetResult();

                    return new ResponseListDto<ChapterItemDto>(chapters);
                }
            );
        }
    }
}