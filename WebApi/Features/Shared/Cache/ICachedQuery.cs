using MediatR;

namespace WebApi.Features.Shared.Cache
{
    public interface ICachedQuery<TResponse> : IRequest<TResponse>, ICachedQuery;

    public interface ICachedQuery
    {
        string Key { get; }
    }
}