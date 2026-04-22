using API.Domains.Processors;
using FastEndpoints;

namespace API.Domains.EndpointGroups
{
    public class ServerGroup : Group
    {
        public ServerGroup()
        {
            Configure("servers/{ServerName}",
                ep =>
                {
                    ep.Description(x => x
                        .WithSummary("Required for all endpoints related to a specific server. The ServerName is the server's domain name, e.g. 'france.x2.france.travian.com'")
                        .WithTags("servers"));
                    ep.PreProcessor<ServerNameValidator>(Order.Before);
                });
        }
    }
}