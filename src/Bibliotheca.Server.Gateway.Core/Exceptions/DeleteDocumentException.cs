using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class DeleteDocumentException : BibliothecaException
    {
        public DeleteDocumentException(string message) : base(message)
        {
        }
    }
}
