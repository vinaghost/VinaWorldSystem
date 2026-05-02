using API.Groups.Player;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Features.GetPlayerVillages
{
    public class GetPlayerVillagesEndpoint(GetPlayerVillagesQuery.Handler handler) :
        Endpoint<
            GetPlayerVillagesRequest,
            Results<Ok<GetPlayerVillagesResponse>, NotFound>>
    {
        public override void Configure()
        {
            Get("{PlayerId}/villages");
            Summary(s => s.Summary = "Get villages by Player ID");
            AllowAnonymous();
            Group<PlayerGroup>();
        }

        public override async Task<Results<Ok<GetPlayerVillagesResponse>, NotFound>> ExecuteAsync(GetPlayerVillagesRequest request, CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(new(request.ServerName, request.PlayerId), cancellationToken);
            if (response is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(new GetPlayerVillagesResponse([.. response]));
        }
    }
}