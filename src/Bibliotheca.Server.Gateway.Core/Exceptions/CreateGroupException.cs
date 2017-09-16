using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class CreateGroupException : BibliothecaException
    {
        public CreateGroupException(string message) : base(message)
        {
        }
    }
}
