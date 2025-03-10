using MediatR;

namespace WebApi.Features.Servers
{
    public static partial class AddServer
    {
        public record Request(string Url) : IRequest<Response>;
    }
}