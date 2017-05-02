using System.Net;
using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class ServiceNotAvailableException : BibliothecaException
    {
        public ServiceNotAvailableException(string message) : base(message, HttpStatusCode.Gone)
        {
        }
    }
}
