using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class RestClient<T>  where T : class
    {
        private readonly string _resourceAddress;

        private readonly string _customAction;
        private readonly IDictionary<string, StringValues> _customHeaders;

        public RestClient(string resourceAddress, IDictionary<string, StringValues> customHeaders, string customAction = null)
        {
            _resourceAddress = resourceAddress;
            _customHeaders = customHeaders;
            _customAction = customAction;
        }

        public async Task<IList<T>> Get()
        {
            HttpClient client = GetClient();

            var requestUri = $"{_resourceAddress}";
            var responseString = await client.GetStringAsync(requestUri);

            var deserializedObject = JsonConvert.DeserializeObject<IList<T>>(responseString);
            return deserializedObject;
        }

        public async Task<T> Get(string id)
        {
            HttpClient client = GetClient();
            var requestUri = $"{_resourceAddress}/{id}";
            if(string.IsNullOrWhiteSpace(_customAction)) 
            {
                requestUri += $"/{_customAction}";
            }
            
            var response = await client.GetAsync(requestUri);
            if(response.StatusCode == HttpStatusCode.OK) 
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var deserializedObject = JsonConvert.DeserializeObject<T>(responseString);
                return deserializedObject;
            }

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var message = await response.Content.ReadAsStringAsync();
            throw new RequestException(response.StatusCode, message);
        }

        public async Task<HttpResponseMessage> Post(T postObject)
        {
            HttpClient client = GetClient();
            var requestUri = $"{_resourceAddress}";

            var serializedObject = JsonConvert.SerializeObject(postObject);
            HttpContent content = new StringContent(serializedObject);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var httpResponseMessage = await client.PostAsync(requestUri, content);

            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> Put(string id, T putObject)
        {
            HttpClient client = GetClient();
            var requestUri = $"{_resourceAddress}/{id}";
            if(string.IsNullOrWhiteSpace(_customAction)) 
            {
                requestUri += $"/{_customAction}";
            }

            var serializedObject = JsonConvert.SerializeObject(putObject);
            HttpContent content = new StringContent(serializedObject);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var httpResponseMessage = await client.PutAsync(requestUri, content);

            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> Delete(string id)
        {
            HttpClient client = GetClient();
            var requestUri = $"{_resourceAddress}/{id}";
            if(string.IsNullOrWhiteSpace(_customAction)) 
            {
                requestUri += $"/{_customAction}";
            }

            var httpResponseMessage = await client.DeleteAsync(requestUri);
            return httpResponseMessage;
        }

        private HttpClient GetClient()
        {
            HttpClient client = new HttpClient();
            RewriteHeader(client, "Authorization");

            return client;
        }

        private void RewriteHeader(HttpClient client, string header)
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