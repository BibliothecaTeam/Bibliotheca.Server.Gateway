using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Bibliotheca.Server.Mvc.Middleware.Diagnostics.Exceptions;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class RestClient<T> : BaseHttpClient  where T : class
    {
        private readonly string _resourceAddress;

        private readonly string _customAction;

        public RestClient(
            HttpClient httpClient,
            string resourceAddress, 
            IDictionary<string, StringValues> customHeaders, 
            string customAction = null) : base(httpClient, customHeaders)
        {
            _resourceAddress = resourceAddress;
            _customAction = customAction;
        }

        public async Task<IList<T>> Get()
        {
            var requestUri = $"{_resourceAddress}";
            var response = await GetAsync(requestUri);

            if(response.StatusCode == HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var deserializedObject = JsonConvert.DeserializeObject<IList<T>>(responseString);
                return deserializedObject;
            }

            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            var message = await response.Content.ReadAsStringAsync();
            throw new RequestException(response.StatusCode, message);
        }

        public async Task<T> Get(string id)
        {
            var requestUri = $"{_resourceAddress}/{id}";
            if(string.IsNullOrWhiteSpace(_customAction)) 
            {
                requestUri += $"/{_customAction}";
            }
            
            var response = await GetAsync(requestUri);
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
            var requestUri = $"{_resourceAddress}";

            var serializedObject = JsonConvert.SerializeObject(postObject);
            HttpContent content = new StringContent(serializedObject);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            var httpResponseMessage = await PostAsync(requestUri, content);
            if(httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
                throw new NotFoundException(responseString);
            }            

            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> Put(string id, T putObject)
        {
            var requestUri = $"{_resourceAddress}/{id}";
            if(!string.IsNullOrWhiteSpace(_customAction)) 
            {
                requestUri += $"/{_customAction}";
            }

            var serializedObject = JsonConvert.SerializeObject(putObject);
            HttpContent content = new StringContent(serializedObject);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var httpResponseMessage = await PutAsync(requestUri, content);
            if(httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
                throw new NotFoundException(responseString);
            }            

            return httpResponseMessage;
        }

        public async Task<HttpResponseMessage> Delete(string id)
        {
            var requestUri = $"{_resourceAddress}/{id}";
            if(!string.IsNullOrWhiteSpace(_customAction)) 
            {
                requestUri += $"/{_customAction}";
            }

            var httpResponseMessage = await DeleteAsync(requestUri);
            if(httpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
                throw new NotFoundException(responseString);
            }

            return httpResponseMessage;
        }
    }
}