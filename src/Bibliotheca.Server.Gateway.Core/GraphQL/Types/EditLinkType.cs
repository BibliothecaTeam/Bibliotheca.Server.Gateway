using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Types
{
    public class EditLinkType : ObjectGraphType<EditLinkDto>, IGraphQLType
    {
        public EditLinkType()
        {
            Field(x => x.BranchName).Description("Name of the branch.");
            Field(x => x.Link).Description("Link to the code.");
        }
    }
}