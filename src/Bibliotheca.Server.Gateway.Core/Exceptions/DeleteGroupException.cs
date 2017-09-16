using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class DeleteGroupException : BibliothecaException
    {
        public DeleteGroupException(string message) : base(message)
        {
        }
    }
}
