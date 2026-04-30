using MySqlConnector;

namespace API.Infrastructure.Services
{
    public class DatabaseService(MySqlDataSource database)
    {
        public async Task<MySqlConnection> OpenConnection(string databaseName)
        {
            ArgumentException.ThrowIfNullOrEmpty(databaseName, nameof(databaseName));

            var connection = await database.OpenConnectionAsync();
            connection.ChangeDatabase(databaseName);
            return connection;
        }
    }
}