using FluentResults;
using MediatR;

namespace WebApi.Features.Tiles
{
    public partial class AddTiles
    {
        public record Command(int ServerId, StreamReader Reader) : IRequest<Result<Response>>;
    }
}