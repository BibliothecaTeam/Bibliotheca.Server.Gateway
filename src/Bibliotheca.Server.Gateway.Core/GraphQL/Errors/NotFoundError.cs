namespace Bibliotheca.Server.Gateway.Core.GraphQL.Errors
{
    public class NotFoundError : GraphQLError
    {
        public NotFoundError(string id) : base(nameof(NotFoundError), $"Resource '{id}' not found.")
        {
        }
    }
}