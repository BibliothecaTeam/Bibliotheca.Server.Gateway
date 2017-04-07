using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;

namespace Bibliotheca.Server.Gateway.Core.Exceptions
{
    public class ProjectNotFoundException : NotFoundException
    {
        public ProjectNotFoundException(string message) : base(message)
        {
        }
    }
}
