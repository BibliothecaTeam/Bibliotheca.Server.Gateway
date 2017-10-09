using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.GraphQL
{
    public class ResponseList<T>
    {
        public IList<T> Data { get; set; }

        public string StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public ResponseList(IList<T> data)
        {
            StatusCode = "Success";
            Data = data;
        }

        public ResponseList(string statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}