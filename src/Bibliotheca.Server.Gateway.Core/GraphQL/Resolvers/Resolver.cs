using System.Collections.Generic;
using Bibliotheca.Server.Gateway.Core.GraphQL.Errors;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Resolvers
{
    public class Resolver
    {
        public Response Response(object data)
        {
            return new Response(data);
        }

        public ResponseList ResponseList(object data)
        {
            return new ResponseList(data);
        }

        public Response Error(GraphQLError error)
        {
            return new Response(error.StatusCode, error.ErrorMessage);
        }

        public Response AccessDeniedError()
        {
            var error = new AccessDeniedError();
            return new Response(error.StatusCode, error.ErrorMessage);
        }

        public Response NotFoundError(string id)
        {
            var error = new NotFoundError(id);
            return new Response(error.StatusCode, error.ErrorMessage);
        }
    }
}