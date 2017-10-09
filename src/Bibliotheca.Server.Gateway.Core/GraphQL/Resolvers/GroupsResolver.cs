using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.GraphQL.Types;
using Bibliotheca.Server.Gateway.Core.Services;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Resolvers
{
    public class GroupsResolver : Resolver, IGroupsResolver
    {
        private readonly IGroupsService _groupsService;

        public GroupsResolver(IGroupsService groupsService)
        {
            _groupsService = groupsService;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            graphQLQuery.Field<ResponseListGraphType<GroupType>>(
                "groups",
                resolve: context => { 
                    var groups = _groupsService.GetGroupsAsync().GetAwaiter().GetResult();
                    return ResponseList(groups);
                }
            );
        }
    }
}