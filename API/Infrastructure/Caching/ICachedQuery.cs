namespace API.Infrastructure.Caching
{
    public interface ICachedQuery
    {
        string CacheKey { get; }

        TimeSpan? Expiration { get; }
    }

    public record DefaultCachedQuery(string CacheKey, TimeSpan? Expiration = null) : ICachedQuery;
}