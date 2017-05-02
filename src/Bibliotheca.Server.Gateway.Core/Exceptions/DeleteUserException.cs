using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class DeleteUserException : BibliothecaException
    {
        public DeleteUserException(string message) : base(message)
        {
        }
    }
}
