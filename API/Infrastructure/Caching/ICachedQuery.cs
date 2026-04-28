namespace API.Infrastructure.Caching
{
    public interface ICachedQuery
    {
        string CacheKey { get; }
        TimeSpan? Expiration { get; }
    }

    public record DefaultCachedQuery(string CacheKey) : ICachedQuery
    {
        public TimeSpan? Expiration => null;
    }
}