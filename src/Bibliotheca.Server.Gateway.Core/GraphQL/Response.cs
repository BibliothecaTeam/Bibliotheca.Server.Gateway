namespace Bibliotheca.Server.Gateway.Core.GraphQL
{
    public class Response<T>
    {
        public T Data { get; set; }

        public string StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public Response(T data)
        {
            StatusCode = "Success";
            Data = data;
        }

        public Response(string statusCode, string errorMessage)
        {
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }
    }
}