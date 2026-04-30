using API.Groups.Player;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Features.GetPlayer
{
    public class GetPlayerEndpoint(GetPlayerQuery.Handler handler) :
        Endpoint<
            GetPlayerRequest,
            Results<Ok<GetPlayerResponse>, NotFound>>
    {
        public override void Configure()
        {
            Get("{PlayerId}");
            Summary(s => s.Summary = "Get player by ID");
            AllowAnonymous();
            Group<PlayerGroup>();
        }

        public override async Task<Results<Ok<GetPlayerResponse>, NotFound>> ExecuteAsync(GetPlayerRequest request, CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(new(request.ServerName, request.PlayerId), cancellationToken);
            if (response is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(new GetPlayerResponse(response));
        }
    }
}