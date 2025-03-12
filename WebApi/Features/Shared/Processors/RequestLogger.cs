using FastEndpoints;

namespace WebApi.Features.Shared.Processors
{
    public class RequestLogger<TRequest> : IPreProcessor<TRequest>
    {
        public async Task PreProcessAsync(IPreProcessorContext<TRequest> ctx, CancellationToken ct)
        {
            var logger = ctx.HttpContext.Resolve<ILogger<TRequest>>();

            logger.LogInformation("request:{FullName} path: {Path}", ctx.Request!.GetType().FullName, ctx.HttpContext.Request.Path);
            await Task.CompletedTask;
        }
    }
}