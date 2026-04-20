using Microsoft.Data.SqlClient;
using System.Data;

namespace SyncSharpServer.Presistence
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly ILogger<DbConnectionFactory> _logger;
        private readonly IConfiguration _configuration;
        public DbConnectionFactory(ILogger<DbConnectionFactory> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<IDbConnection> GetOpenConnection(CancellationToken cancellationToken)
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("SharpConnection");
                var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);
                return connection;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to open DB connection.");
                throw;
            }
        }
    }
}
