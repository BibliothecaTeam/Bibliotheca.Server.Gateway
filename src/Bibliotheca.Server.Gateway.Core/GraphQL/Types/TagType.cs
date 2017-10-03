using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Types
{
    public class TagType : StringGraphType, IGraphQLType
    {
        public TagType()
        {
            Name = "tags";
            Description = "Collection og tag names.";
        }
    }
}