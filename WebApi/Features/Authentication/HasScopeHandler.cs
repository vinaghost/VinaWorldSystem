﻿using Microsoft.AspNetCore.Authorization;

namespace WebApi.Features.Authentication
{
    public class HasScopeHandler : AuthorizationHandler<HasScopeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
        {
            if (!context.User.HasClaim(c => c.Type == "scope" && c.Issuer == requirement.Issuer))
            {
                return Task.CompletedTask;
            }

            var claim = context.User.FindFirst(c => c.Type == "scope" && c.Issuer == requirement.Issuer);
            if (claim == null)
            {
                return Task.CompletedTask;
            }
            var scopes = claim.Value.Split(' ');

            if (!scopes.Any(s => s == requirement.Scope))
            {
                return Task.CompletedTask;
            }

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}