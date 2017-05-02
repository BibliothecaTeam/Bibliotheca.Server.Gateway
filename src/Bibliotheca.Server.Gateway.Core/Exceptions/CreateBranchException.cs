using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class CreateBranchException : BibliothecaException
    {
        public CreateBranchException(string message) : base(message)
        {
        }
    }
}
