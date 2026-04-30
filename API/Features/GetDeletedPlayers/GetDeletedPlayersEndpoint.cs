using API.Groups.Player;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Features.GetDeletedPlayers
{
    public class GetDeletedPlayersEndpoint(GetDeletedPlayersQuery.Handler handler) :
        Endpoint<
            GetDeletedPlayersRequest,
            Results<Ok<GetDeletedPlayersResponse>, NotFound>>
    {
        public override void Configure()
        {
            Get("deleted/{Date}");
            Summary(s => s.Summary = "Get deleted players by date");
            AllowAnonymous();
            Group<PlayerGroup>();
        }

        public override async Task<Results<Ok<GetDeletedPlayersResponse>, NotFound>> ExecuteAsync(GetDeletedPlayersRequest request, CancellationToken cancellationToken)
        {
            if (request.Date > DateTime.UtcNow)
            {
                return TypedResults.Ok(new GetDeletedPlayersResponse([]));
            }

            var response = await handler.HandleAsync(new(request.ServerName, request.Date), cancellationToken);
            return TypedResults.Ok(new GetDeletedPlayersResponse([.. response]));
        }
    }
}