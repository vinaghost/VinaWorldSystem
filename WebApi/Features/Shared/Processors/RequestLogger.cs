using FastEndpoints;
using Microsoft.AspNetCore.Authentication;

namespace WebApi.Features.Shared.Processors
{
    public class RequestLogger<TRequest> : IPreProcessor<TRequest>
    {
        public async Task PreProcessAsync(IPreProcessorContext<TRequest> ctx, CancellationToken ct)
        {
            var logger = ctx.HttpContext.Resolve<ILogger<TRequest>>();

            logger.LogInformation("request:{FullName} path: {Path}", ctx.Request!.GetType().FullName, ctx.HttpContext.Request.Path);
            var accessToken = await ctx.HttpContext.GetTokenAsync("access_token");

            var claims = ctx.HttpContext.User.Claims.ToArray();

            _ = 2;


        }
    }
}