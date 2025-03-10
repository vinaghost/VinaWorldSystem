using FastEndpoints;
using WebApi.Features.Servers;

namespace WebApi.Features.Tiles
{
    public class TileGroup : SubGroup<ServerGroup>
    {
        public TileGroup()
        {
            Configure("{ServerId}/tiles", ep =>
            {
                ep.Description(x => x
                  .WithTags("tiles"));
            });
        }
    }
}