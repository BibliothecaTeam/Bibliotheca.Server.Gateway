using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class BranchConfigurationFileNotExistsException : BibliothecaException
    {
        public BranchConfigurationFileNotExistsException(string message) : base(message)
        {
        }
    }
}