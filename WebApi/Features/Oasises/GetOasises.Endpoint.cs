using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApi.Features.Shared.Errors;

namespace WebApi.Features.Oasises
{
    public partial class GetOasises
    {
        public class Endpoint(IMediator mediator) : Endpoint<Request, Results<Ok<Response>, NotFound, BadRequest>>
        {
            private readonly IMediator _mediator = mediator;

            public override void Configure()
            {
                Get("/");
                Permissions("read:tiles");
                Group<OasisGroup>();
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