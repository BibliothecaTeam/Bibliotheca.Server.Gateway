using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class DeleteProjectException : BibliothecaException
    {
        public DeleteProjectException(string message) : base(message)
        {
        }
    }
}
