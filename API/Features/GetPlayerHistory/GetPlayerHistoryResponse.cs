namespace API.Features.GetPlayerHistory
{
    public record GetPlayerHistoryResponse(GetPlayerHistoryQuery.Response Players, List<GetVillagesHistoryQuery.Response> Villages);
}