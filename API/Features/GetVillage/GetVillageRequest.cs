using API.Groups.Server;

namespace API.Features.GetVillage
{
    public record GetVillageRequest(string ServerName, int VillageId) : ServerNameRequest(ServerName);
}