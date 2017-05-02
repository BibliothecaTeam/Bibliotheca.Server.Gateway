using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class CreateProjectException : BibliothecaException
    {
        public CreateProjectException(string message) : base(message)
        {
        }
    }
}
