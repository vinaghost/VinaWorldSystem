using FastEndpoints;

namespace API.Features.GetServers
{
    public class GetServersEndpoint(GetServersQuery.Handler handler)
        : EndpointWithoutRequest<
            GetServersResponse>
    {
        public override void Configure()
        {
            Get("/servers");
            Summary(s => s.Summary = "Get all servers");
            Description(b => b.WithTags("Servers"));
            AllowAnonymous();
        }

        public override async Task<GetServersResponse> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(new(), cancellationToken);
            return new GetServersResponse([.. response]);
        }
    }
}