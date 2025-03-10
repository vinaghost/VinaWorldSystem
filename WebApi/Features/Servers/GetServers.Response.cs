using WebApi.Features.Servers.Shared;

namespace WebApi.Features.Servers
{
    public partial class GetServers
    {
        public record Response(List<ServerDto> Results);
    }
}