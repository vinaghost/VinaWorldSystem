using API.Infrastructure.Services;
using Dapper;
using Immediate.Handlers.Shared;

namespace API.Features.GetPlayer
{
    [Handler]
    public static partial class GetPlayerQuery
    {
        public sealed record Query(string ServerName, int PlayerId);
        public record Response()
        {
            public required int PlayerId { get; init; }
            public required string PlayerName { get; init; }
            public required int AllianceId { get; init; }
            public required string AllianceName { get; init; }
            public required int VillageCount { get; init; }
            public required int Population { get; init; }
        }

        private static async ValueTask<Response?> HandleAsync(
            Query query,
            DatabaseService databaseService,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var connection = await databaseService.OpenConnection(query.ServerName);
            var response = await connection.QueryFirstOrDefaultAsync<Response>("""
SELECT p.Id AS PlayerId, p.Name AS PlayerName, a.Id AS AllianceId, a.Name AS AllianceName, p.Population, p.VillageCount
FROM Players p JOIN Alliances a ON p.AllianceId = a.Id
WHERE p.Id = @PlayerId
""", new { query.PlayerId });
            return response;
        }
    }
}