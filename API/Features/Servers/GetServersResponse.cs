namespace API.Features.Servers
{
    public record GetServersResponse(string ServerName, DateTime LastUpdate, int VillageCount, int PlayerCount, int AllianceCount);
}