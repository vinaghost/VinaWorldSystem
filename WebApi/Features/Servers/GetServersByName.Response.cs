using WebApi.Features.Servers.Shared;

namespace WebApi.Features.Servers
{
    public partial class GetServersByName
    {
        public record Response(List<ServerDto> Results);
    }
}