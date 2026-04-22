namespace API.Features.GetPlayer
{
    public record GetPlayerResponse(int PlayerId, string PlayerName, int AllianceId, string AllianceName, int VillageCount, int Population);
}