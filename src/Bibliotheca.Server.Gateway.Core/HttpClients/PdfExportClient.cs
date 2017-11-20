using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.Exceptions;
using Bibliotheca.Server.Gateway.Core.Parameters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Bibliotheca.Server.Gateway.Core.HttpClients
{
    public class PdfExportClient : IPdfExportClient
    {
        private readonly string _baseAddress;

        private readonly ILogger _logger;

        private readonly HttpClient _httpClient;

        private readonly ApplicationParameters _applicationParameters;

        public PdfExportClient(
            string baseAddress, 
            ILogger<NightcrawlerClient> logger,
            HttpClient httpClient,
            IOptions<ApplicationParameters> applicationParameters) 
        {
            _baseAddress = baseAddress;
            _logger = logger;
            _httpClient = httpClient;
            _applicationParameters = applicationParameters.Value;
        }

        public async Task<HttpResponseMessage> Post(string markdown)
        {
            AssertIfServiceNotAlive();

            var requestUri = Path.Combine(_baseAddress, $"transform");
            _logger.LogInformation($"PdfExport client request (POST): {requestUri}");

            var client = GetClient();
            HttpContent content = new StringContent(markdown);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

            var httpResponseMessage = await client.PostAsync(requestUri, content);
            return httpResponseMessage;
        }

        private BaseHttpClient GetClient()
        {
            var customHeaders = new HttpContextHeaders();
            customHeaders.Headers.Add("Authorization", $"SecureToken {_applicationParameters.SecureToken}");

            var baseClient = new BaseHttpClient(_httpClient, customHeaders);
            return baseClient;
        }

        private void AssertIfServiceNotAlive()
        {
            if(!IsServiceAlive()) 
            {
                throw new ServiceNotAvailableException($"Microservice with tag 'pdfexport' is not running!");
            }
        }

        private bool IsServiceAlive()
        {
            return !string.IsNullOrWhiteSpace(_baseAddress);
        }
    }
}