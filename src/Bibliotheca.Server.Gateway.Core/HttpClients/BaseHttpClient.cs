using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class BaseHttpClient
    {
        private readonly IDictionary<string, StringValues> _customHeaders;

        private readonly HttpClient _httpClient;

        public BaseHttpClient(HttpClient httpClient, IDictionary<string, StringValues> customHeaders)
        {
            _httpClient = httpClient;
            _customHeaders = customHeaders;
        }

        public Task<HttpResponseMessage> GetAsync(string uri)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            RewriteHeader(httpRequestMessage, "Authorization");
            
            return _httpClient.SendAsync(httpRequestMessage);
        }

        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, uri);
            RewriteHeader(httpRequestMessage, "Authorization");

            httpRequestMessage.Content = content;

            return _httpClient.SendAsync(httpRequestMessage);
        }

        public Task<HttpResponseMessage> PutAsync(string uri, HttpContent content)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Put, uri);
            RewriteHeader(httpRequestMessage, "Authorization");
            
            httpRequestMessage.Content = content;

            return _httpClient.SendAsync(httpRequestMessage);
        }

        public Task<HttpResponseMessage> DeleteAsync(string uri)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);
            RewriteHeader(httpRequestMessage, "Authorization");
            
            return _httpClient.SendAsync(httpRequestMessage);
        }

        protected void RewriteHeader(HttpRequestMessage requestMessage, string header)
        {
            if (_customHeaders != null)
            {
                if(_customHeaders.ContainsKey(header))
                {
                    var value = _customHeaders[header];
                    requestMessage.Headers.Add(header, value as IList<string>);
                }
            }
        }
    }
}