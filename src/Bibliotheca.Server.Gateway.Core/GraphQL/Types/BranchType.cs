using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Types
{
    public class BranchType : ObjectGraphType<BranchDto>, IGraphQLType
    {
        public BranchType()
        {
            Field(x => x.Name).Description("Name of the branch.");
            Field(x => x.MkDocsYaml).Description("Definition of the branch.");
        }
    }
}