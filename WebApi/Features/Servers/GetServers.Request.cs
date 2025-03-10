using MediatR;

namespace WebApi.Features.Servers
{
    public partial class GetServers
    {
        public record Request() : IRequest<Response>;
    }
}