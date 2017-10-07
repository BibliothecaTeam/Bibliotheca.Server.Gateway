namespace Bibliotheca.Server.Gateway.Core.GraphQL.Resolvers
{
    public interface IResolver
    {
        void Resolve(GraphQLQuery graphQLQuery);
    }
}