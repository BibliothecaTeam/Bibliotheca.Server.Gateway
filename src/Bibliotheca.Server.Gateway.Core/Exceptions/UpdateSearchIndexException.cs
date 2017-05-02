using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class UpdateSearchIndexException : BibliothecaException
    {
        public UpdateSearchIndexException(string message) : base(message)
        {
        }
    }
}
