using API.Features.Shared;

namespace API.Features.GetNewVillages
{
    public record GetNewVillagesRequest(string ServerName, DateTime Date) : ServerNameRequest(ServerName);
}