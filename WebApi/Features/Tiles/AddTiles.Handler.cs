using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Entities;
using WebApi.Features.Shared.Errors;

namespace WebApi.Features.Tiles
{
    public partial class AddTiles
    {
        public class Handler(AppDbContext context) : IRequestHandler<Request, Result<Response>>
        {
            private readonly AppDbContext _context = context;

            public async Task<Result<Response>> Handle(Request request, CancellationToken cancellationToken)
            {
                var server = await _context.Servers
                    .AnyAsync(x => x.Id == request.ServerId);

                if (!server)
                {
                    return new ItemNotFound("server");
                }

                var tiles = request.Tiles.Select(t => new Tile
                {
                    ServerId = request.ServerId,
                    MapId = t.MapId,
                    X = t.X,
                    Y = t.Y,
                    Type = t.Type,
                    Status = t.Status
                });
                _context.Tiles.AddRange(tiles);
                var count = await _context.SaveChangesAsync(cancellationToken);
                return new Response(count);
            }
        }
    }
}