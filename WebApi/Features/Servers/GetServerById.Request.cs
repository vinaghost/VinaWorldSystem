using FluentResults;
using MediatR;

namespace WebApi.Features.Servers
{
    public partial class GetServerById
    {
        public record Request(int Id) : IRequest<Result<string>>;
    }
}