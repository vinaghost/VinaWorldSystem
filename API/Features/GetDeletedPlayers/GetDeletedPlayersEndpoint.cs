using API.Domains.EndpointGroups;
using API.Features.Shared;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Features.GetDeletedPlayers
{
    public record GetDeletedPlayersRequest(string ServerName, DateTime Date) : ServerNameRequest(ServerName);

    public class GetDeletedPlayersEndpoint(GetDeletedPlayersQuery.Handler handler) : Endpoint<GetDeletedPlayersRequest, Results<Ok<List<GetDeletedPlayersResponse>>, NotFound>>
    {
        public override void Configure()
        {
            Get("/players/deleted/{Date}");
            AllowAnonymous();
            Group<ServerGroup>();
        }

        public override async Task<Results<Ok<List<GetDeletedPlayersResponse>>, NotFound>> ExecuteAsync(GetDeletedPlayersRequest request, CancellationToken cancellationToken)
        {
            if (request.Date > DateTime.UtcNow)
            {
                return TypedResults.Ok(new List<GetDeletedPlayersResponse>());
            }

            var result = await handler.HandleAsync(new(request.ServerName, request.Date), cancellationToken);
            var response = result.Select(r => new GetDeletedPlayersResponse(
                r.PlayerId,
                r.PlayerName,
                r.DeletedDate
            )).ToList();
            return TypedResults.Ok(response);
        }
    }
}