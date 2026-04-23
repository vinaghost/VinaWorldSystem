using API.Features.Shared;

namespace API.Features.GetPlayerHistory
{
    public record GetPlayerHistoryRequest(string ServerName, int PlayerId) : ServerNameRequest(ServerName);
}
