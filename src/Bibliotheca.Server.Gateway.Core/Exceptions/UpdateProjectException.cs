using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class UpdateProjectException : BibliothecaException
    {
        public UpdateProjectException(string message) : base(message)
        {
        }
    }
}
