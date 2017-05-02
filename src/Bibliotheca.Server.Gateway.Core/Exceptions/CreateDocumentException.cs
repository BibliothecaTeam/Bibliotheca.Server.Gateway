using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class CreateDocumentException : BibliothecaException
    {
        public CreateDocumentException(string message) : base(message)
        {
        }
    }
}
