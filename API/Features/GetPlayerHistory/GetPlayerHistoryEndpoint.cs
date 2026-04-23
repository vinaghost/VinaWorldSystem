using API.Domains.EndpointGroups;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Features.GetPlayerHistory
{
    public class GetPlayerHistoryEndpoint(GetPlayerHistoryQuery.Handler playerHandler, GetVillagesHistoryQuery.Handler villageHandler) : Endpoint<
        GetPlayerHistoryRequest,
        Results<Ok<GetPlayerHistoryResponse>, NotFound>>
    {
        public override void Configure()
        {
            Get("/players/history/{PlayerId}");
            AllowAnonymous();
            Group<ServerGroup>();
        }

        public override async Task<Results<Ok<GetPlayerHistoryResponse>, NotFound>> ExecuteAsync(
            GetPlayerHistoryRequest request, CancellationToken cancellationToken)
        {
            var playerResponse = await playerHandler.HandleAsync(new(request.ServerName, request.PlayerId), cancellationToken);
            if (playerResponse is null)
            {
                return TypedResults.NotFound();
            }
            var villagesResponse = await villageHandler.HandleAsync(new(request.ServerName, request.PlayerId), cancellationToken);
            if (villagesResponse is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(new GetPlayerHistoryResponse(playerResponse, [.. villagesResponse]));
        }
    }
}