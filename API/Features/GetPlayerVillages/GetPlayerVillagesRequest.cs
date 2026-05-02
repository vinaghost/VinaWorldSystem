using API.Groups.Server;

namespace API.Features.GetPlayerVillages
{
    public record GetPlayerVillagesRequest(string ServerName, int PlayerId) : ServerNameRequest(ServerName);
}
