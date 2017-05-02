using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class UpdateBranchException : BibliothecaException
    {
        public UpdateBranchException(string message) : base(message)
        {
        }
    }
}
