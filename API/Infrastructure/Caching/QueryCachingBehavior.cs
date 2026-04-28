using Immediate.Handlers.Shared;

namespace API.Infrastructure.Caching
{
    public sealed class QueryCachingBehavior<TRequest, TResponse>(CacheService cacheService)
        : Behavior<TRequest, TResponse> where TRequest : ICachedQuery
    {
        public override async ValueTask<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken)
        {
            var cacheKey = request.CacheKey;
            var cachedValue = await cacheService.GetOrCreateAsync(
                cacheKey,
                async token => await Next(request, token),
                request.Expiration,
                cancellationToken);

            return cachedValue!;
        }
    }
}