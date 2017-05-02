using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class UpdateDocumentException : BibliothecaException
    {
        public UpdateDocumentException(string message) : base(message)
        {
        }
    }
}
