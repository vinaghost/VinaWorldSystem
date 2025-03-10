using FluentResults;
using MediatR;
using WebApi.Features.Tiles.Shared;

namespace WebApi.Features.Tiles
{
    public partial class AddTiles
    {
        public record Request(int ServerId, List<TileDto> Tiles) : IRequest<Result<Response>>;
    }
}