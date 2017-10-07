using Bibliotheca.Server.Gateway.Core.GraphQL.Types;
using Bibliotheca.Server.Gateway.Core.Services;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Resolvers
{
    public class TagsResolver : ITagsResolver
    {
        private readonly ITagsService _tagsService;

        public TagsResolver(ITagsService tagsService)
        {
            _tagsService = tagsService;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            graphQLQuery.Field<ListGraphType<TagType>>(
                "tags",
                resolve: context => { 
                    return _tagsService.GetAvailableTagsAsync();
                }
            );
        }
    }
}