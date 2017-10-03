using System;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.GraphQL;
using Bibliotheca.Server.Mvc.Middleware.Authorization;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;

namespace Bibliotheca.Server.Gateway.Api.Controllers
{
    /// <summary>
    /// Controller for GraphQL.
    /// </summary>
    [UserAuthorize]
    [Route("api/graphql")]
    public class GraphQLController : Controller
    {
        private readonly GraphQLQuery _graphQLQuery;
        private readonly IDocumentExecuter _documentExecuter;
        private readonly ISchema _schema;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="graphQLQuery">GraphQL for Bibliotheca query.</param>
        /// <param name="documentExecuter">GraphQL document executor.</param>
        /// <param name="schema">GraphQL schema.</param>
        public GraphQLController(GraphQLQuery graphQLQuery, IDocumentExecuter documentExecuter, ISchema schema)
        {
            _graphQLQuery = graphQLQuery;
            _documentExecuter = documentExecuter;
            _schema = schema;
        }

        /// <summary>
        /// Main endpoint for retrievieng data by GrapQL query language.
        /// </summary>
        /// <param name="query">Query.</param>
        /// <returns>Data retrieved by query.</returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQLParameter query)
        {
            var executionOptions = new ExecutionOptions { Schema = _schema, Query = query.Query };
            var result = await _documentExecuter.ExecuteAsync(executionOptions).ConfigureAwait(false);

            if (result.Errors?.Count > 0)
            {
                return BadRequest(result.Errors);
            }

            return Ok(result);
        }
    }
}