using System.Net;
using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class RequestException : BibliothecaException
    {
        HttpStatusCode _statusCode;

        public RequestException(HttpStatusCode statusCode, string message) : base(message)
        {
            _statusCode = statusCode;
        }
    }
}
