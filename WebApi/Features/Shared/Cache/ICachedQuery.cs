using FluentResults;
using MediatR;

namespace WebApi.Features.Shared.Cache
{
    public interface ICachedQuery<TResponse> : IRequest<Result<TResponse>>, ICachedQuery;

    public interface ICachedQuery
    {
        string Key { get; }
    }
}