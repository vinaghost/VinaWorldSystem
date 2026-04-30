using API.Groups.Server;
using FastEndpoints;

namespace API.Groups.Village
{
    public class VillageGroup : SubGroup<ServerGroup>
    {
        public VillageGroup()
        {
            Configure("villages",
                ep =>
                {
                    ep.Description(x => x
                        .WithDescription("Villages related")
                        .WithTags("Villages"));
                });
        }
    }
}