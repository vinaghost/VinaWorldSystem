using API.Infrastructure.Services;
using Dapper;
using Immediate.Handlers.Shared;

namespace API.Features.GetPlayerHistory
{
    [Handler]
    public static partial class GetVillagesHistoryQuery
    {
        public sealed record Query(string ServerName, int PlayerId);
        public record Response
        {
            public required int VillageId { get; init; }
            public required string VillageName { get; init; }
            public required int X { get; init; }
            public required int Y { get; init; }
            public required bool IsCapital { get; init; }
            public ICollection<VillageHistory> History { get; init; } = [];
        }
        public record VillageHistory
        {
            public required DateTime UpdateDate { get; init; }
            public required int PlayerId { get; init; }
            public required string PlayerName { get; init; }
            public required bool ChangePlayer { get; init; }
            public required int Population { get; init; }
            public required int ChangePopulation { get; init; }
        }

        private static async ValueTask<IEnumerable<Response>> HandleAsync(
            Query query,
            DatabaseService databaseService,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var connection = await databaseService.OpenConnection(query.ServerName);
            var statement = """
SELECT
    v.Id AS VillageId,
    v.Name AS VillageName,
    v.X,
    v.Y,
    v.IsCapital,
    h.Date AS UpdateDate,
    h.PlayerId,
    p.Name AS PlayerName,
    h.ChangePlayer,
    h.Population,
    h.ChangePopulation
FROM Villages v
JOIN VillagesHistory h on v.Id = h.VillageId
JOIN Players p on h.PlayerId = p.Id
WHERE h.PlayerId = @PlayerId AND h.Date > DATE_ADD(CURDATE(), INTERVAL -7 DAY)
ORDER BY h.Id DESC;
""";
            var villageDictionary = new Dictionary<int, Response>();

            var response = await connection.QueryAsync<Response, VillageHistory, Response>(statement, (village, history) =>
            {
                if (!villageDictionary.TryGetValue(village.VillageId, out var currentVillage))
                {
                    currentVillage = village;
                    villageDictionary.Add(currentVillage.VillageId, currentVillage);
                }
                currentVillage.History.Add(history);
                return currentVillage;
            }, splitOn: "UpdateDate", param: new { query.PlayerId });

            return villageDictionary.Values;
        }
    }
}