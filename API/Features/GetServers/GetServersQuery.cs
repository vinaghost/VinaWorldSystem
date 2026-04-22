using API.Infrastructure.Services;
using Dapper;
using Immediate.Handlers.Shared;

namespace API.Features.GetServers
{
    [Handler]
    public static partial class GetServersQuery
    {
        public sealed record Query;
        public record Response()
        {
            public required string ServerName { get; init; }
            public required DateTime LastUpdate { get; init; }
            public required int VillageCount { get; init; }
            public required int PlayerCount { get; init; }
            public required int AllianceCount { get; init; }
        }

        private static async ValueTask<IEnumerable<Response>> HandleAsync(
            Query _,
            DatabaseService databaseService,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var connection = await databaseService.OpenConnection("Servers");
            var response = await connection.QueryAsync<Response>("SELECT LastUpdate, Url AS ServerName, VillageCount, PlayerCount, AllianceCount FROM Servers");
            return response;
        }
    }
}