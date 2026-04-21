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
    }
}