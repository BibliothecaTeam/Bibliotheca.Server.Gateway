using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Types
{
    public class ChapterItemType : ObjectGraphType<ChapterItemDto>, IGraphQLType
    {
        public ChapterItemType()
        {
            Field(x => x.Name).Description("The chapter name.");
            Field(x => x.Url).Description("The url to the chapter.");
            
            Field<ListGraphType<ChapterItemType>>(
                "children",
                "Subchapters.",
                resolve: context => context.Source.Children
            );
        }
    }
}