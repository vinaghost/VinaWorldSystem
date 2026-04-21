using API.Infrastructure.Services;
using FastEndpoints;

namespace API.Features.Servers
{
    public class GetServersEndpoint(GetServersQuery.Handler handler) : EndpointWithoutRequest<List<GetServersResponse>>
    {
        public override void Configure()
        {
            Get("/servers");
            AllowAnonymous();
        }

        public override async Task<List<GetServersResponse>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(new(), cancellationToken);
            return [.. response.Select(s => new GetServersResponse(s.ServerName, s.LastUpdate, s.VillageCount, s.PlayerCount, s.AllianceCount))];
        }
    }
}