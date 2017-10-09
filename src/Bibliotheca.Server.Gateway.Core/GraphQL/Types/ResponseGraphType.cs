using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using GraphQL;
using GraphQL.Builders;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Types
{
    public class ResponseGraphType<TGraphType, TSourceType> : ObjectGraphType<Response<TSourceType>> where TGraphType : GraphType
    {
        public ResponseGraphType()
        {
            Name = $"Response{typeof(TGraphType).Name}";

            Field(x => x.StatusCode, nullable: true).Description("Status code of the request.");
            Field(x => x.ErrorMessage, nullable: true).Description("Error message if requests fails.");

            Field<TGraphType>(
                "data",
                "Project data returned by query.",
                resolve: context => context.Source.Data
            );
        }
    }
}