using API.Infrastructure.Services;
using Dapper;
using Immediate.Handlers.Shared;

namespace API.Features.GetNewVillages
{
    [Handler]
    public static partial class GetNewVillagesQuery
    {
        public sealed record Query(string ServerName, DateTime Date);
        public record Response()
        {
            public required int PlayerId { get; init; }
            public required string PlayerName { get; init; }
            public required int AllianceId { get; init; }
            public required string AllianceName { get; init; }
            public required int X { get; init; }
            public required int Y { get; init; }
            public required int Tribe { get; init; }
            public required int Population { get; init; }
            public required bool IsCapital { get; init; }
            public required bool IsCity { get; init; }
            public required bool IsHarbor { get; init; }
        }

        private static async ValueTask<IEnumerable<Response>> HandleAsync(
            Query query,
            DatabaseService databaseService,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var connection = await databaseService.OpenConnection(query.ServerName);
            var statement = """
WITH new_villages AS (
    SELECT
        VillageId,
        COUNT(*) AS record_count
    FROM
        VillagesHistory
    WHERE
        date <= DATE(@Date)
    GROUP BY
        VillageId
    HAVING
        record_count = 1
)
SELECT
    p.Id         AS PlayerId,
    p.Name       AS PlayerName,
    a.Id         AS AllianceId,
    a.Name       AS AllianceName,
    v.X,
    v.Y,
    v.Tribe,
    v.Population,
    v.IsCapital,
    v.IsCity,
    v.IsHarbor
FROM
    new_villages n
    JOIN Villages v   ON n.VillageId = v.Id
    JOIN Players p    ON v.PlayerId = p.Id
    JOIN Alliances a  ON p.AllianceId = a.Id
WHERE
    v.Population <> 0
ORDER BY
    v.Population DESC;
""";
            var response = await connection.QueryAsync<Response>(statement, new { Date = query.Date.ToString("yyyy-MM-dd") });
            return response;
        }
    }
}