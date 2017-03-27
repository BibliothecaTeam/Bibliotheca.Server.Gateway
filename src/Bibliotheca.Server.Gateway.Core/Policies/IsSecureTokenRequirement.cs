using Microsoft.AspNetCore.Authorization;

namespace Bibliotheca.Server.Gateway.Core.Policies
{
    public class IsSecureTokenRequirement : IAuthorizationRequirement
    {
    }
}