using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public abstract class BaseHttpClient
    {
        private readonly IDictionary<string, StringValues> _customHeaders;

        public BaseHttpClient(IDictionary<string, StringValues> customHeaders)
        {
            _customHeaders = customHeaders;
        }

        protected HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            RewriteHeader(client, "Authorization");

            return client;
        }

        protected void RewriteHeader(HttpClient client, string header)
        {
            if (_customHeaders != null)
            {
                if(_customHeaders.ContainsKey(header))
                {
                    var value = _customHeaders[header];
                    client.DefaultRequestHeaders.Add(header, value as IList<string>);
                }
            }
        }
    }
}