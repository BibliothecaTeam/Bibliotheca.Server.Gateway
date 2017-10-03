using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Types
{
    public class ContactPersonType : ObjectGraphType<ContactPersonDto>, IGraphQLType
    {
        public ContactPersonType()
        {
            Field(x => x.Name).Description("The user name.");
            Field(x => x.Email).Description("The user email.");
        }
    }
}