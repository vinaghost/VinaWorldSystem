using API.Groups.Server;

namespace API.Features.GetDeletedPlayers
{
    public record GetDeletedPlayersRequest(string ServerName, DateTime Date) : ServerNameRequest(ServerName);
}