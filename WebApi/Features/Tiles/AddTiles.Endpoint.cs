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
                Permissions("write:tiles");
                AllowFileUploads(dontAutoBindFormData: true);
                Group<TileGroup>();
            }

            public override async Task<Results<Ok<Response>, NotFound, BadRequest>> ExecuteAsync(Request request, CancellationToken cancellationToken)
            {
                if (Files.Count <= 0)
                {
                    return TypedResults.BadRequest();
                }

                var file = Files[0];
                var reader = new StreamReader(file.OpenReadStream());
                var result = await _mediator.Send(new Command(request.ServerId, reader), cancellationToken);
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