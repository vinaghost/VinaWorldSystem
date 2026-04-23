using API.Features.Shared;

namespace API.Features.GetDeletedPlayers
{
    public record GetDeletedPlayersRequest(string ServerName, DateTime Date) : ServerNameRequest(ServerName);
}