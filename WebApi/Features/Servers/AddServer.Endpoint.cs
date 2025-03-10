using FastEndpoints;
using MediatR;

namespace WebApi.Features.Servers
{
    public static partial class AddServer
    {
        public class Endpoint(IMediator mediator) : Endpoint<Request, Response>
        {
            private readonly IMediator _mediator = mediator;

            public override void Configure()
            {
                Post("/");
                AllowAnonymous();
                Group<ServerGroup>();
            }

            public override async Task HandleAsync(Request request, CancellationToken cancellationToken)
            {
                var result = await _mediator.Send(request, cancellationToken);
                await SendAsync(result);
            }
        }
    }
}