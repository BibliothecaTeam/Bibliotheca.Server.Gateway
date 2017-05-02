using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class DeleteSearchIndexException : BibliothecaException
    {
        public DeleteSearchIndexException(string message) : base(message)
        {
        }
    }
}
