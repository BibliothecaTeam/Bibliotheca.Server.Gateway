namespace Bibliotheca.Server.Gateway.Core.GraphQL
{
    public class Response
    {
        public object Data { get; set; }

        public string StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public Response(object data)
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