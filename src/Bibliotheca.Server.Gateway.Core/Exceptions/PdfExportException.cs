using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class PdfExportException : BibliothecaException
    {
        public PdfExportException(string message) : base(message)
        {
        }
    }
}
