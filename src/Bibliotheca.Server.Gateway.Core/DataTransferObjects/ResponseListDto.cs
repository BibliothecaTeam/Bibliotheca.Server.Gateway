using System.Collections.Generic;

namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class ResponseListDto<T>
    {
        public IList<T> Data { get; set; }

        public string StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public ResponseListDto(IList<T> data)
        {
            StatusCode = "Success";
            Data = data;
        }

        public ResponseListDto(string statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}