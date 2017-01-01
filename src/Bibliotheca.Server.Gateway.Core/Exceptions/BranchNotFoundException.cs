namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class BranchNotFoundException : BibliothecaException
    {
        public BranchNotFoundException(string message) : base(message)
        {
        }
    }
}
