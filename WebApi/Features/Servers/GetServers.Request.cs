using WebApi.Features.Shared.Cache;

namespace WebApi.Features.Servers
{
    public partial class GetServers
    {
        public record Request() : ICachedQuery<Response>
        {
            public string Key => $"{nameof(GetServers)}";
        }
    }
}