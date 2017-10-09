using System;
using System.Linq.Expressions;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Types
{
    public class ResponseGraphType<TSourceType> : ObjectGraphType<ResponseDto<TSourceType>>
    {
        public ResponseGraphType()
        {
            Field(x => x.StatusCode, nullable: true).Description("Status code of the request.");
            Field(x => x.ErrorMessage, nullable: true).Description("Error message if requests fails.");
        }        
    }
}