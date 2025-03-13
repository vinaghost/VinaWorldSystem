using CsvHelper;
using CsvHelper.Configuration.Attributes;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WebApi.Context;
using WebApi.Entities;
using WebApi.Features.Shared.Errors;

namespace WebApi.Features.Tiles
{
    public partial class AddTiles
    {
        public class Handler(AppDbContext context) : IRequestHandler<Command, Result<Response>>
        {
            public class RawTile
            {
                [Name("x")]
                public int X { get; set; }

                [Name("y")]
                public int Y { get; set; }

                [Name("tile_type")]
                public string TileType { get; set; } = "";

                [Name("tile_type_2")]
                public string TileType2 { get; set; } = "";

                [Name("player_name")]
                public string PlayerName { get; set; } = "";

                [Name("village_name")]
                public string VillageName { get; set; } = "";

                [Name("map_id")]
                public int MapId { get; set; }
            }

            private readonly AppDbContext _context = context;

            public async Task<Result<Response>> Handle(Command request, CancellationToken cancellationToken)
            {
                var server = await _context.Servers
                    .AnyAsync(x => x.Id == request.ServerId);

                if (!server)
                {
                    return new ItemNotFound("server");
                }

                using var csv = new CsvReader(request.Reader, CultureInfo.InvariantCulture);
                var records = csv.GetRecords<RawTile>();
                var tiles = records.Select(t => new Tile
                {
                    ServerId = request.ServerId,
                    MapId = t.MapId,
                    X = t.X,
                    Y = t.Y,
                    Type = t.TileType,
                    Status = t.TileType2
                });
                _context.Tiles.AddRange(tiles);
                var count = await _context.SaveChangesAsync(cancellationToken);
                return new Response(count);
            }
        }
    }
}