using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class UpdateUserException : BibliothecaException
    {
        public UpdateUserException(string message) : base(message)
        {
        }
    }
}
