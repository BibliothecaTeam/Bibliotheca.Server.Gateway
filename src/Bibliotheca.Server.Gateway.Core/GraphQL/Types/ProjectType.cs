using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Services;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Types
{
    public class ProjectType : ObjectGraphType<ProjectDto>, IGraphQLType
    {
        public ProjectType(IBranchesService branchesService)
        {
            Field(x => x.Id).Description("The id of the project.");
            Field(x => x.Name, nullable: true).Description("The name of the project.");
            Field(x => x.Description, nullable: true).Description("The description of the project.");
            Field(x => x.DefaultBranch, nullable: true).Description("Project's default branch.");
            Field(x => x.Group).Description("The group of the project.");
            Field(x => x.ProjectSite).Description("The site of the project.");
            Field(x => x.AccessToken).Description("The access token of the project.");
            Field(x => x.IsAccessLimited).Description("The information about access to the project.");
            
            Field<ListGraphType<StringGraphType>>(
                "visibleBranches",
                resolve: context => context.Source.VisibleBranches
            );

            Field<ListGraphType<StringGraphType>>(
                "tags",
                resolve: context => context.Source.Tags
            );

            Field<ListGraphType<ContactPersonType>>(
                "contactPeople",
                resolve: context => context.Source.ContactPeople
            );

            Field<ListGraphType<EditLinkType>>(
                "editLinks",
                resolve: context => context.Source.EditLinks
            );

            Field<ListGraphType<BranchType>>(
                "branches",
                resolve: context => branchesService.GetBranchesAsync(context.Source.Id)
            );
        }
    }
}