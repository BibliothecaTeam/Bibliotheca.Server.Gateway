using System;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class BibliothecaException : Exception
    {
        public BibliothecaException() : base()
        {
        }

        public BibliothecaException(string message) : base(message)
        {
        }
    }
}
