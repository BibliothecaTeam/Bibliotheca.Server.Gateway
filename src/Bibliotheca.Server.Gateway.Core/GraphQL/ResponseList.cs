using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.GraphQL
{
    public class ResponseList
    {
        public object Data { get; set; }

        public string StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public ResponseList(object data)
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