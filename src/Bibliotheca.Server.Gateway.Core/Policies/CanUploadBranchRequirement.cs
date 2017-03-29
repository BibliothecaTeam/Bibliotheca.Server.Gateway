using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class CanUploadBranchRequirement : IAuthorizationRequirement
    {
        public IHeaderDictionary Headers;

        public CanUploadBranchRequirement(IHeaderDictionary headers)
        {
            Headers = headers;
        }
    }
}