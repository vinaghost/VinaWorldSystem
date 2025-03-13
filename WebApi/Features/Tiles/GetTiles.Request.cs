using WebApi.Features.Shared.Cache;

namespace WebApi.Features.Tiles
{
    public partial class GetTiles
    {
        public record Request(int ServerId) : ICachedQuery<Response>
        {
            public string Key => $"{nameof(GetTiles)}_{ServerId}";
        }
    }
}