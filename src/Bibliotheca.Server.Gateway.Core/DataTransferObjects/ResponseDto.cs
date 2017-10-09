namespace Bibliotheca.Server.Gateway.Core.DataTransferObjects
{
    public class ResponseDto<T>
    {
        public T Data { get; set; }

        public string StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public ResponseDto(T data)
        {
            StatusCode = "Success";
            Data = data;
        }

        public ResponseDto(string statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}