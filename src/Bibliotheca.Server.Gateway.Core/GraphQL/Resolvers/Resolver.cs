using System.Collections.Generic;
using Bibliotheca.Server.Gateway.Core.GraphQL.Errors;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Resolvers
{
    public class Resolver
    {
        public Response<T> Response<T>(T data)
        {
            return new Response<T>(data);
        }

        public ResponseList<T> ResponseList<T>(IList<T> data)
        {
            return new ResponseList<T>(data);
        }

        public Response<T> Error<T>(GraphQLError error)
        {
            return new Response<T>(error.StatusCode, error.ErrorMessage);
        }

        public Response<T> AccessDeniedError<T>()
        {
            var error = new AccessDeniedError();
            return new Response<T>(error.StatusCode, error.ErrorMessage);
        }

        public Response<T> NotFoundError<T>(string id)
        {
            var error = new NotFoundError(id);
            return new Response<T>(error.StatusCode, error.ErrorMessage);
        }
    }
}