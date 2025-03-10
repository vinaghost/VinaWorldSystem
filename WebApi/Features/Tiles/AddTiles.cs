using FastEndpoints;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Entities;
using WebApi.Features.Shared.Errors;

namespace WebApi.Features.Tiles
{
    public class AddTiles
    {
        public record TileDto(int X, int Y, int MapId, string Type, string Status);
        public record Response(int Count);
        public record Request(int ServerId, List<TileDto> Tiles) : IRequest<Result<Response>>;

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

        public class Endpoint(IMediator mediator) : Endpoint<Request, Results<Ok<Response>, NotFound, BadRequest>>
        {
            private readonly IMediator _mediator = mediator;

            public override void Configure()
            {
                Get("{Id}");
                AllowAnonymous();
                Group<TileGroup>();
            }

            public override async Task<Results<Ok<Response>, NotFound, BadRequest>> ExecuteAsync(Request request, CancellationToken cancellationToken)
            {
                var result = await _mediator.Send(request, cancellationToken);
                if (result.IsFailed)
                {
                    if (result.HasError<ItemNotFound>())
                    {
                        return TypedResults.NotFound();
                    }
                    return TypedResults.BadRequest();
                }

                return TypedResults.Ok(result.Value);
            }
        }
    }
}