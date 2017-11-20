using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public interface IHttpContextHeaders
    {
        IDictionary<string, StringValues> Headers { get; set; }
    }
}