using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using System.Diagnostics;
using WebApi.Features.Shared.Cache;

namespace WebApi.Features.Shared.Behaviors
{
    public sealed class QueryCachingPipelineBehavior<TRequest, TResponse>(IDistributedCache cacheService, ILogger<QueryCachingPipelineBehavior<TRequest, TResponse>> logger, ActivitySource activitySource) : IPipelineBehavior<TRequest, TResponse> where TRequest : ICachedQuery<TResponse>
    {
        private readonly IDistributedCache _cacheService = cacheService;
        private readonly ILogger<QueryCachingPipelineBehavior<TRequest, TResponse>> _logger = logger;
        private readonly ActivitySource _activitySource = activitySource;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cacheKey = request.Key;
            var response = await _cacheService.GetOrSetAsync<TResponse>(cacheKey,
                async () =>
                {
                    using var activity = _activitySource.StartActivity("Query execution");
                    _logger.LogInformation("Cache key {CacheKey} miss. Execute query.", cacheKey);
                    return await next();
                },
                cancellationToken);
            return response!;
        }
    }
}