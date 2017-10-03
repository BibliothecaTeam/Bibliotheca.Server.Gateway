using System;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL
{
    public class GraphQLSchema : Schema
    {
        public GraphQLSchema(Func<Type, GraphType> resolveType)
            : base(resolveType)
        {
            Query = (GraphQLQuery)resolveType(typeof(GraphQLQuery));
        }
    }
}