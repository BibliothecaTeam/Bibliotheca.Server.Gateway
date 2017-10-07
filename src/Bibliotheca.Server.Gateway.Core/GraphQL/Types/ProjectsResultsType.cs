using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Types
{
    public class ProjectsResultsType : ObjectGraphType<FilteredResutsDto<ProjectDto>>, IGraphQLType
    {
        public ProjectsResultsType()
        {
            Field(x => x.AllResults).Description("Number of all projects.");

            Field<ListGraphType<ProjectType>>(
                "results",
                "Filtered projects list.",
                resolve: context => context.Source.Results
            );
        }
    }
}