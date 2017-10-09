using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
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
            graphQLQuery.Field<ResponseListGraphType<StringGraphType, string>>(
                "tags",
                resolve: context => { 
                    var tags = _tagsService.GetAvailableTagsAsync().GetAwaiter().GetResult();
                    return new ResponseListDto<string>(tags);
                }
            );
        }
    }
}