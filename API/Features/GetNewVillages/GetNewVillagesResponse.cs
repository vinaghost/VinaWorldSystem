namespace API.Features.GetNewVillages
{
    public record GetNewVillagesResponse(
        int PlayerId,
        string PlayerName,
        int AllianceId,
        string AllianceName,
        int X,
        int Y,
        int Tribe,
        int Population,
        bool IsCapital,
        bool IsCity,
        bool IsHarbor
    );
}