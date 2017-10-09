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
    public class ResponseListGraphType<TGraphType> : ObjectGraphType<ResponseList> where TGraphType : GraphType
    {
        public ResponseListGraphType()
        {
            Name = $"ResponseList{typeof(TGraphType).Name}";

            Field(x => x.StatusCode, nullable: true).Description("Status code of the request.");
            Field(x => x.ErrorMessage, nullable: true).Description("Error message if requests fails.");

            Field<ListGraphType<TGraphType>>(
                "data",
                "Project data returned by query.",
                resolve: context => context.Source.Data
            );
        }
    }
}