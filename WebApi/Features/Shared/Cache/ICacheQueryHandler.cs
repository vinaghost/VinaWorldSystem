using FluentResults;
using MediatR;

namespace WebApi.Features.Shared.Cache
{
    public interface ICacheQueryHandler<TRequest, TResponse> : IRequestHandler<TRequest, Result<TResponse>>
        where TRequest : ICachedQuery<TResponse>;
}