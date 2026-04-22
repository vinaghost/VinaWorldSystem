using API.Domains.EndpointGroups;
using API.Features.GetServers;
using API.Features.Shared;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Features.GetPlayer
{
    public record GetPlayerRequest(string ServerName, int PlayerId) : ServerNameRequest(ServerName);

    public class GetPlayerEndpoint(GetPlayerQuery.Handler handler) : Endpoint<GetPlayerRequest, Results<Ok<GetPlayerResponse>, NotFound>>
    {
        public override void Configure()
        {
            Get("/players/{PlayerId}");
            AllowAnonymous();
            Group<ServerGroup>();
        }

        public override async Task<Results<Ok<GetPlayerResponse>, NotFound>> ExecuteAsync(GetPlayerRequest request, CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(new GetPlayerQuery.Query(request.ServerName, request.PlayerId), cancellationToken);
            if (response is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(new GetPlayerResponse(response.PlayerId, response.PlayerName, response.AllianceId, response.AllianceName, response.VillageCount, response.Population));
        }
    }
}