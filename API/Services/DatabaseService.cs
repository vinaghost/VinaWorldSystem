using API.Endpoints.Servers;
using Dapper;
using MySqlConnector;
using System.Data;

namespace API.Services
{
    public class DatabaseService(MySqlDataSource database)
    {
        public async Task<List<Response>> GetServers()
        {
            await using var connection = await database.OpenConnectionAsync();
            connection.ChangeDatabase("Servers");

            var response = await connection.QueryAsync<Response>("SELECT LastUpdate, VillageCount, PlayerCount, AllianceCount FROM Servers");
            return [.. response];
        }
    }
}