namespace API.Features.GetDeletedPlayers
{
    public record GetDeletedPlayersResponse(
        int PlayerId,
        string PlayerName,
        DateTime DeletedDate
    );
}