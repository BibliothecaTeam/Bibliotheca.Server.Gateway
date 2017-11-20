using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class HttpContextHeaders : IHttpContextHeaders
    {
        private static AsyncLocal<IDictionary<string, StringValues>> _headers = new AsyncLocal<IDictionary<string, StringValues>>();

        public IDictionary<string, StringValues> Headers
        {
            get
            {
                return _headers.Value;
            }
            set
            {
                _headers.Value = value;
            }
        }
    }
}