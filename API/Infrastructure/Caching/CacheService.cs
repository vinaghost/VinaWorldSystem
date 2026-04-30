using Microsoft.Extensions.Caching.Memory;

namespace API.Infrastructure.Caching
{
    public class CacheService(IMemoryCache memoryCache)
    {
        private static readonly TimeSpan DefaultExpiration = TimeSpan.FromMinutes(5);

        public async Task<T?> GetOrCreateAsync<T>(
            string key,
            Func<CancellationToken, Task<T>> factory,
            TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
        {
            var result = await memoryCache.GetOrCreateAsync(
                key,
                entry =>
                {
                    entry.SetAbsoluteExpiration(expiration ?? DefaultExpiration);
                    return factory(cancellationToken);
                });
            return result;
        }
    }
}