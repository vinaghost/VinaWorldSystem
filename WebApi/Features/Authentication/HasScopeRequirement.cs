using Microsoft.AspNetCore.Authorization;

namespace WebApi.Features.Authentication
{
    public record HasScopeRequirement(string Scope, string Issuer) : IAuthorizationRequirement;
}