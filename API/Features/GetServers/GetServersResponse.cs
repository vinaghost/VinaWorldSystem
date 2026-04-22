namespace API.Features.GetServers
{
    public record GetServersResponse(string ServerName, DateTime LastUpdate, int VillageCount, int PlayerCount, int AllianceCount);
}