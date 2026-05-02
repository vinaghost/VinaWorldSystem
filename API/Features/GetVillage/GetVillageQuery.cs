using API.Infrastructure.Caching;
using API.Infrastructure.Services;
using Dapper;
using Immediate.Handlers.Shared;

namespace API.Features.GetVillage
{
    [Handler]
    public static partial class GetVillageQuery
    {
        public sealed record Query(string ServerName, int VillageId) : DefaultCachedQuery($"{nameof(GetVillageQuery)}_{ServerName}_{VillageId}");
        public record Response()
        {
            public required int PlayerId { get; init; }
            public required string PlayerName { get; init; }
            public required int AllianceId { get; init; }
            public required string AllianceName { get; init; }
            public required int VillageId { get; init; }
            public required int MapId { get; init; }
            public required string VillageName { get; init; }
            public required int X { get; init; }
            public required int Y { get; init; }
            public required int Tribe { get; init; }
            public required int Population { get; init; }
            public required string Region { get; init; }
            public required bool IsCapital { get; init; }
            public required bool IsCity { get; init; }
            public required bool IsHarbor { get; init; }
            public required int VictoryPoints { get; init; }
        }

        private static async ValueTask<Response?> HandleAsync(
            Query query,
            DatabaseService databaseService,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var connection = await databaseService.OpenConnection(query.ServerName);
            var statement = """
SELECT
    p.Id           AS PlayerId,
    p.Name         AS PlayerName,
    a.Id           AS AllianceId,
    a.Name         AS AllianceName,
    v.Id           AS VillageId,
    v.MapId,
    v.Name         AS VillageName,
    v.X,
    v.Y,
    v.Tribe,
    v.Population,
    v.Region,
    v.IsCapital,
    v.IsCity,
    v.IsHarbor,
    v.VictoryPoints
FROM
    Villages v
    JOIN Players p   ON v.PlayerId = p.Id
    JOIN Alliances a ON p.AllianceId = a.Id
WHERE
    v.Id = @VillageId
""";
            var response = await connection.QueryFirstOrDefaultAsync<Response>(statement, new { query.VillageId });
            return response;
        }
    }
}