using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebApi.Features.Servers
{
    public partial class GetServers
    {
        public class Endpoint(IMediator mediator) : EndpointWithoutRequest<Results<Ok<Response>, NotFound, BadRequest>>
        {
            private readonly IMediator _mediator = mediator;

            public override void Configure()
            {
                Get("/");
                AllowAnonymous();
                Group<ServerGroup>();
            }

            public override async Task<Results<Ok<Response>, NotFound, BadRequest>> ExecuteAsync(CancellationToken cancellationToken)
            {
                var result = await _mediator.Send(new Request(), cancellationToken);
                return TypedResults.Ok(result);
            }
        }
    }
}