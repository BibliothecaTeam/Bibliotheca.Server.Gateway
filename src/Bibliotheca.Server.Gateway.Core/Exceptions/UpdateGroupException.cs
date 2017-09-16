using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class UpdateGroupException : BibliothecaException
    {
        public UpdateGroupException(string message) : base(message)
        {
        }
    }
}