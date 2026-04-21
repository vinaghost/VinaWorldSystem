using API.Features.Servers;
using Dapper;
using MySqlConnector;
using System.Data;

namespace API.Infrastructure.Services
{
    public class DatabaseService(MySqlDataSource database)
    {
        public async Task<MySqlConnection> OpenConnection(string databaseName)
        {
            var connection = await database.OpenConnectionAsync();
            connection.ChangeDatabase(databaseName);
            return connection;
        }

        public async Task<List<Response>> GetServers()
        {
            await using var connection = await OpenConnection("Servers");

            var response = await connection.QueryAsync<Response>("SELECT LastUpdate, VillageCount, PlayerCount, AllianceCount FROM Servers");
            return [.. response];
        }
    }
}