namespace Bibliotheca.Server.Gateway.Core.GraphQL.Errors
{
    public class FileTypeNotSupportedError : GraphQLError
    {
        public FileTypeNotSupportedError(string fileType) : base(nameof(FileTypeNotSupportedError), $"File type '{fileType}' is not supported. You can download only markdown files.")
        {
        }
    }
}