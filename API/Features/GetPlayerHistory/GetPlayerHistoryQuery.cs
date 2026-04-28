using API.Infrastructure.Caching;
using API.Infrastructure.Services;
using Dapper;
using Immediate.Handlers.Shared;

namespace API.Features.GetPlayerHistory
{
    [Handler]
    public static partial class GetPlayerHistoryQuery
    {
        public sealed record Query(string ServerName, int PlayerId) : DefaultCachedQuery($"{nameof(GetPlayerHistoryQuery)}_{ServerName}_{PlayerId}");
        public record Response
        {
            public required int PlayerId { get; init; }
            public required string PlayerName { get; init; }
            public ICollection<PlayerHistory> History { get; init; } = [];
        }

        public record Player
        {
            public required int PlayerId { get; init; }
            public required string PlayerName { get; init; }
        }
        public record PlayerHistory
        {
            public required DateTime UpdateDate { get; init; }
            public required int AllianceId { get; init; }
            public required string AllianceName { get; init; }
            public required bool ChangeAlliance { get; init; }
            public required int Population { get; init; }
            public required int ChangePopulation { get; init; }
        }

        private static async ValueTask<Response> HandleAsync(
            Query query,
            DatabaseService databaseService,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var connection = await databaseService.OpenConnection(query.ServerName);
            var statement = """
SELECT
    Id   AS PlayerId,
    Name AS PlayerName
FROM
    Players
WHERE
    Id = @PlayerId;

SELECT
    h.Date            AS UpdateDate,
    h.AllianceId,
    a.Name            AS AllianceName,
    h.ChangeAlliance,
    h.Population,
    h.ChangePopulation
FROM
    PlayersHistory h
    JOIN Alliances a ON h.AllianceId = a.Id
WHERE
    h.PlayerId = @PlayerId
    AND h.Date > DATE_ADD(CURDATE(), INTERVAL -7 DAY)
ORDER BY
    h.Date DESC;
""";
            using var multi = await connection.QueryMultipleAsync(statement, new { query.PlayerId });
            var player = multi.ReadFirst<Player>();
            var playerHistory = multi.Read<PlayerHistory>().ToList();
            return
                new Response
                {
                    PlayerId = player.PlayerId,
                    PlayerName = player.PlayerName,
                    History = playerHistory
                };
        }
    }
}