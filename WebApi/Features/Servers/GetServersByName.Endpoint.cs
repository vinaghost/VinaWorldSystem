using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApi.Features.Servers
{
    public partial class GetServersByName
    {
        public class Endpoint(IMediator mediator) : Endpoint<Request, Results<Ok<Response>, NotFound, BadRequest>>
        {
            private readonly IMediator _mediator = mediator;

            public override void Configure()
            {
                Get("/name/{SearchTerm}");
                AllowAnonymous();
                Group<ServerGroup>();
            }

            public override async Task<Results<Ok<Response>, NotFound, BadRequest>> ExecuteAsync(Request request, CancellationToken cancellationToken)
            {
                var result = await _mediator.Send(request, cancellationToken);
                return TypedResults.Ok(result);
            }
        }
    }
}