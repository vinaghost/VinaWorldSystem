using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApi.Features.Shared.Errors;

namespace WebApi.Features.Tiles
{
    public partial class AddTiles
    {
        public class Endpoint(IMediator mediator) : Endpoint<Request, Results<Ok<Response>, NotFound, BadRequest>>
        {
            private readonly IMediator _mediator = mediator;

            public override void Configure()
            {
                Get("/");
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