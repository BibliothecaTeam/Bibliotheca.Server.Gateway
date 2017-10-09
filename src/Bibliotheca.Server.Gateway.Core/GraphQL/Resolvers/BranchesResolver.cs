using System.Collections.Generic;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.GraphQL.Types;
using Bibliotheca.Server.Gateway.Core.Services;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Resolvers
{
    public class BranchesResolver : IBranchesResolver
    {
        private readonly IBranchesService _branchesService;

        public BranchesResolver(IBranchesService branchesService)
        {
            _branchesService = branchesService;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            graphQLQuery.Field<ResponseListGraphType<BranchType, ExtendedBranchDto>>(
                "branches",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "projectId", Description = "id of the project" }
                ),
                resolve: context => { 
                    var projectId = context.GetArgument<string>("projectId");
                    var list = _branchesService.GetBranchesAsync(projectId).GetAwaiter().GetResult();

                    return new ResponseListDto<ExtendedBranchDto>(list);
                }
            );

            graphQLQuery.Field<ResponseGraphType<BranchType, BranchDto>>(
                "branch",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "projectId", Description = "id of the project" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "branchName", Description = "name of the branch" }
                ),
                resolve: context => { 
                    var projectId = context.GetArgument<string>("projectId");
                    var branchName = context.GetArgument<string>("branchName");
                    var branch = _branchesService.GetBranchAsync(projectId, branchName).GetAwaiter().GetResult();

                    return new ResponseDto<BranchDto>(branch);
                }
            );
        }
    }
}