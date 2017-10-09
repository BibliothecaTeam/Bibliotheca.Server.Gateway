using System;
using Bibliotheca.Server.Gateway.Core.DataTransferObjects;
using Bibliotheca.Server.Gateway.Core.GraphQL.Errors;
using Bibliotheca.Server.Gateway.Core.GraphQL.Types;
using Bibliotheca.Server.Gateway.Core.Services;
using GraphQL.Types;

namespace Bibliotheca.Server.Gateway.Core.GraphQL.Resolvers
{
    public class DocumentsResolver : Resolver, IDocumentsResolver
    {
        private readonly IDocumentsService _documentsService;
        private readonly IMarkdownService _markdownService;

        public DocumentsResolver(IDocumentsService documentsService, IMarkdownService markdownService)
        {
            _documentsService = documentsService;
            _markdownService = markdownService;
        }

        public void Resolve(GraphQLQuery graphQLQuery)
        {
            graphQLQuery.Field<ResponseGraphType<StringGraphType>>(
                "document",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "projectId", Description = "id of the project" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "branchName", Description = "name of the branch" },
                    new QueryArgument<NonNullGraphType<StringGraphType>> { Name = "fileUri", Description = "url of the file" }
                ),
                resolve: context => { 
                    var projectId = context.GetArgument<string>("projectId");
                    var branchName = context.GetArgument<string>("branchName");
                    var fileUri = context.GetArgument<string>("fileUri");
                    
                    var document = _documentsService.GetDocumentAsync(projectId, branchName, fileUri).GetAwaiter().GetResult();
                    if(document == null) 
                    {
                        return NotFoundError(fileUri);
                    }

                    byte[] content = document.Content;
                    string contentType = document.ConentType;

                    if(document.ConentType == "text/markdown")
                    {
                        var markdown = System.Text.Encoding.UTF8.GetString(document.Content);
                        var html = _markdownService.ConvertToHtml(markdown);

                        content = System.Text.Encoding.UTF8.GetBytes(html);
                        var base64Html = Convert.ToBase64String(content);

                        return Response(base64Html);
                    }

                    return Error(new FileTypeNotSupportedError(document.ConentType));
                }
            );
        }
    }
}