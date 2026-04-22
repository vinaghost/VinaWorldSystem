using API.Domains.EndpointGroups;
using API.Features.Shared;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace API.Features.GetNewVillages
{
    public record GetNewVillagesRequest(string ServerName, DateTime Date) : ServerNameRequest(ServerName);

    public class GetNewVillagesEndpoint(GetNewVillagesQuery.Handler handler) : Endpoint<GetNewVillagesRequest, Results<Ok<List<GetNewVillagesResponse>>, NotFound>>
    {
        public override void Configure()
        {
            Get("/villages/new/{Date}");
            AllowAnonymous();
            Group<ServerGroup>();
        }

        public override async Task<Results<Ok<List<GetNewVillagesResponse>>, NotFound>> ExecuteAsync(GetNewVillagesRequest request, CancellationToken cancellationToken)
        {
            if (request.Date > DateTime.UtcNow)
            {
                return TypedResults.Ok(new List<GetNewVillagesResponse>());
            }

            var result = await handler.HandleAsync(new(request.ServerName, request.Date), cancellationToken);
            var response = result.Select(r => new GetNewVillagesResponse(
                r.PlayerId,
                r.PlayerName,
                r.AllianceId,
                r.AllianceName,
                r.X,
                r.Y,
                r.Tribe,
                r.Population,
                r.IsCapital,
                r.IsCity,
                r.IsHarbor
            )).ToList();
            return TypedResults.Ok(response);
        }
    }
}