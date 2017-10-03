using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Types
{
    public class GroupType : ObjectGraphType<GroupDto>, IGraphQLType
    {
        public GroupType()
        {
            Field(x => x.Name).Description("The group name.");
            Field(x => x.SvgIcon).Description("The icon of group.");
        }
    }
}