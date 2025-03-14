using CsvHelper;
using CsvHelper.Configuration.Attributes;
using EFCore.BulkExtensions;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using WebApi.Context;
using WebApi.Entities;
using WebApi.Features.Shared.Errors;

namespace WebApi.Features.Tiles
{
    public partial class AddTiles
    {
        public class Handler(AppDbContext context, ILogger<Handler> logger, ActivitySource activitySource) : IRequestHandler<Command, Result<Response>>
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
            private readonly ILogger<Handler> _logger = logger;
            private readonly ActivitySource _activitySource = activitySource;

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
                var tiles = records
                    .Select(t => new Tile
                    {
                        ServerId = request.ServerId,
                        MapId = t.MapId,
                        X = t.X,
                        Y = t.Y,
                        Type = t.TileType,
                        Status = t.TileType2
                    })
                    .ToList();

                _logger.LogInformation("Found {Count} tiles in the file", tiles.Count);

                if (tiles.Count <= 0)
                {
                    return new Response(0);
                }

                using (var activity = _activitySource.StartActivity("Delete old tiles"))
                {
                    await _context.Tiles
                        .Where(t => t.ServerId == request.ServerId)
                        .ExecuteDeleteAsync(cancellationToken);
                }

                using (var activity = _activitySource.StartActivity("Insert new tiles"))
                {
                    await _context.BulkInsertAsync(tiles, cancellationToken: cancellationToken);
                }

                return new Response(tiles.Count);
            }
        }
    }
}