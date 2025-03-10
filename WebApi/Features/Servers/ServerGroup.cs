using FastEndpoints;

namespace WebApi.Features.Servers
{
    public class ServerGroup : Group
    {
        public ServerGroup()
        {
            Configure("servers", ep =>
            {
                ep.Description(x => x
                  .WithTags("servers"));
            });
        }
    }
}