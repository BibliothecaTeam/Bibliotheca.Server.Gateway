using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class DeleteBranchException : BibliothecaException
    {
        public DeleteBranchException(string message) : base(message)
        {
        }
    }
}
