using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Types
{
    public class ProjectResponseType : ResponseGraphType<ProjectDto>, IGraphQLType
    {
        public ProjectResponseType()
        {
            Field<ProjectType>(
                "data",
                "Project data returned by query.",
                resolve: context => context.Source.Data
            );
        }
    }
}