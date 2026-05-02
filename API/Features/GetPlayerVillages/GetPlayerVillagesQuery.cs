using API.Infrastructure.Caching;
using API.Infrastructure.Services;
using Dapper;
using Immediate.Handlers.Shared;

namespace API.Features.GetPlayerVillages
{
    [Handler]
    public static partial class GetPlayerVillagesQuery
    {
        public sealed record Query(string ServerName, int PlayerId) : DefaultCachedQuery($"{nameof(GetPlayerVillagesQuery)}_{ServerName}_{PlayerId}");
        public record Response()
        {
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

        private static async ValueTask<IEnumerable<Response>> HandleAsync(
            Query query,
            DatabaseService databaseService,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var connection = await databaseService.OpenConnection(query.ServerName);
            var statement = """
SELECT
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
WHERE
    v.PlayerId = @PlayerId
""";
            var response = await connection.QueryAsync<Response>(statement, new { query.PlayerId });
            return response;
        }
    }
}