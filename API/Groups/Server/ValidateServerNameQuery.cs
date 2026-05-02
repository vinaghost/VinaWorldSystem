using API.Infrastructure.Caching;
using API.Infrastructure.Services;
using Dapper;
using Immediate.Handlers.Shared;

namespace API.Groups.Server
{
    [Handler]
    public static partial class ValidateServerNameQuery
    {
        public sealed record Query(string ServerName) : DefaultCachedQuery($"{nameof(ValidateServerNameQuery)}_{ServerName}");

        private static async ValueTask<bool> HandleAsync(
            Query query,
            DatabaseService databaseService,
            CancellationToken cancellationToken
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            await using var connection = await databaseService.OpenConnection("Servers");
            var statement = """
SELECT
    id AS ServerId
FROM
    Servers
WHERE
    Url = @ServerName
""";
            var response = await connection.QueryFirstOrDefaultAsync<int?>(statement, new { query.ServerName });
            return response.HasValue;
        }
    }
}