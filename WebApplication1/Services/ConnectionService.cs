using Microsoft.Data.SqlClient;

namespace DatabaseComparer.Services
{
    public class ConnectionService
    {
        public string BuildConnectionString(string server, string database, string authType, string? username, string? password)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = string.IsNullOrWhiteSpace(database) ? "master" : database,
                TrustServerCertificate = true
            };

            if (authType == "SQL")
            {
                builder.UserID = username ?? string.Empty;
                builder.Password = password ?? string.Empty;
            }
            else
            {
                builder.IntegratedSecurity = true;
            }

            return builder.ConnectionString;
        }

        public (bool Success, string Message) TestConnection(string connectionString)
        {
            try
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                return (true, "Bağlantı başarılı!");
            }
            catch (Exception ex)
            {
                return (false, $"Bağlantı başarısız: {ex.Message}");
            }
        }

        public List<string> GetDatabaseList(string connectionString)
        {
            var databases = new List<string>();

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            using var command = new SqlCommand("SELECT name FROM sys.databases WHERE database_id > 4 ORDER BY name", connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                databases.Add(reader.GetString(0));
            }

            return databases;
        }
    }
}