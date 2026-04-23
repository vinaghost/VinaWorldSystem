using API.Infrastructure.Services;
using Dapper;
using Immediate.Handlers.Shared;

namespace API.Features.GetDeletedPlayers
{
    [Handler]
    public static partial class GetDeletedPlayersQuery
    {
        public sealed record Query(string ServerName, DateTime Date);
        public record Response()
        {
            public required int PlayerId { get; init; }
            public required string PlayerName { get; init; }
            public required DateTime DeletedDate { get; init; }
        }

        private static async ValueTask<IEnumerable<Response>> HandleAsync(
            Query query,
            DatabaseService databaseService,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var connection = await databaseService.OpenConnection(query.ServerName);
            var response = await connection.QueryAsync<Response>("""
WITH delete_players AS (
    SELECT
        h.PlayerId,
        ROW_NUMBER() OVER (PARTITION BY PlayerId ORDER BY h.Id DESC) AS row_index,
        h.Date
    FROM
        Players p
        JOIN PlayersHistory h ON p.id = h.PlayerId
    WHERE
        p.VillageCount = 0
        AND h.Population = 0
)
SELECT
    p.Id        AS PlayerId,
    p.Name      AS PlayerName,
    a.id        AS AllianceId,
    a.Name      AS AllainceName,
    dp.Date     AS DeletedDate
FROM
    delete_players dp
    JOIN Players p   ON dp.PlayerId = p.Id
    JOIN Alliances a ON p.AllianceId = a.Id
WHERE
    dp.row_index = 1
    AND dp.Date = Date(@Date);
""", new { Date = query.Date.ToString("yyyy-MM-dd") });
            return response;
        }
    }
}