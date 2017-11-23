using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class HttpContextHeaders : IHttpContextHeaders
    {
        private IDictionary<string, StringValues> _headers = new Dictionary<string, StringValues>();

        public IDictionary<string, StringValues> Headers
        {
            get
            {
                return _headers;
            }
            set
            {
                _headers = value;
            }
        }
    }
}