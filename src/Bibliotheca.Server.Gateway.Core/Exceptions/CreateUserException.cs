using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class CreateUserException : BibliothecaException
    {
        public CreateUserException(string message) : base(message)
        {
        }
    }
}
