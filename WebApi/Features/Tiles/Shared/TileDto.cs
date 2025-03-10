namespace WebApi.Features.Tiles.Shared
{
    public record TileDto(int X, int Y, int MapId, string Type, string Status);
}