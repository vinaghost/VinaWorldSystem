using API.Services;
using FastEndpoints;

namespace API.Features.Servers
{
    public class Endpoint(DatabaseService databaseService) : EndpointWithoutRequest<List<Response>>
    {
        public override void Configure()
        {
            Get("/servers");
            AllowAnonymous();
        }

        public override async Task<List<Response>> ExecuteAsync(CancellationToken ct)
        {
            return await databaseService.GetServers();
        }
    }
}