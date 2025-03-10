using MediatR;

namespace WebApi.Features.Servers
{
    public partial class GetServersByName
    {
        public record Request(string SearchTerm) : IRequest<Response>;
    }
}