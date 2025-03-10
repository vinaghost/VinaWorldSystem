using FastEndpoints;

namespace WebApi.Features.Tiles
{
    public class TileGroup : Group
    {
        public TileGroup()
        {
            Configure("tiles", ep =>
            {
                ep.Description(x => x
                  .WithTags("tiles"));
            });
        }
    }
}