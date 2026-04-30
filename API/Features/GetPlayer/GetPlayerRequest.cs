using API.Groups.Server;

namespace API.Features.GetPlayer
{
    public record GetPlayerRequest(string ServerName, int PlayerId) : ServerNameRequest(ServerName);
}