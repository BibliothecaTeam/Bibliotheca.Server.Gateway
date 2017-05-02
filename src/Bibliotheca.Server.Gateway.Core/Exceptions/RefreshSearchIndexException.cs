using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class RefreshSearchIndexException : BibliothecaException
    {
        public RefreshSearchIndexException(string message) : base(message)
        {
        }
    }
}
