using API.Features.GetVillage;
using API.Groups.Village;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Features.GetVillage
{
    public class GetVillageEndpoint(GetVillageQuery.Handler handler) :
        Endpoint<
            GetVillageRequest,
            Results<Ok<GetVillageResponse>, NotFound>>
    {
        public override void Configure()
        {
            Get("{VillageId}");
            Summary(s => s.Summary = "Get Village by ID");
            AllowAnonymous();
            Group<VillageGroup>();
        }

        public override async Task<Results<Ok<GetVillageResponse>, NotFound>> ExecuteAsync(GetVillageRequest request, CancellationToken cancellationToken)
        {
            var response = await handler.HandleAsync(new(request.ServerName, request.VillageId), cancellationToken);
            if (response is null)
            {
                return TypedResults.NotFound();
            }
            return TypedResults.Ok(new GetVillageResponse(response));
        }
    }
}