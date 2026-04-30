using FastEndpoints;

namespace API.Groups.Server
{
    public class ServerGroup : Group
    {
        public ServerGroup()
        {
            Configure("servers/{ServerName}",
                ep =>
                {
                    ep.Description(b => b
                        .Produces(400)
                        .Produces(404));

                    ep.Options(b => b
                        .AddEndpointFilter<ServerNameFilter>());
                });
        }
    }
}