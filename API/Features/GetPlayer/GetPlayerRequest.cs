using API.Features.Shared;

namespace API.Features.GetPlayer
{
    public record GetPlayerRequest(string ServerName, int PlayerId) : ServerNameRequest(ServerName);
}