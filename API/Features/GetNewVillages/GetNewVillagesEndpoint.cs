using API.Groups.Village;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Features.GetNewVillages
{
    public class GetNewVillagesEndpoint(GetNewVillagesQuery.Handler handler) :
        Endpoint<
            GetNewVillagesRequest,
            Results<Ok<GetNewVillagesResponse>, NotFound>>
    {
        public override void Configure()
        {
            Get("new/{Date}");
            Summary(s => s.Summary = "Get new villages by date");
            AllowAnonymous();
            Group<VillageGroup>();
        }

        public override async Task<Results<Ok<GetNewVillagesResponse>, NotFound>> ExecuteAsync(GetNewVillagesRequest request, CancellationToken cancellationToken)
        {
            if (request.Date > DateTime.UtcNow)
            {
                return TypedResults.Ok(new GetNewVillagesResponse([]));
            }

            var response = await handler.HandleAsync(new(request.ServerName, request.Date), cancellationToken);
            return TypedResults.Ok(new GetNewVillagesResponse([.. response]));
        }
    }
}