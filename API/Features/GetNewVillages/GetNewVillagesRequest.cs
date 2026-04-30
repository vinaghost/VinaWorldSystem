using API.Groups.Server;

namespace API.Features.GetNewVillages
{
    public record GetNewVillagesRequest(string ServerName, DateTime Date) : ServerNameRequest(ServerName);
}