using System.Data;
using Microsoft.Data.SqlClient;
using CalculateFunding.Common.Sql.Interfaces;
using CalculateFunding.Common.Utility;

namespace CalculateFunding.Common.Sql
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly ISqlSettings _settings;

        public SqlConnectionFactory(ISqlSettings settings)
        {
            Guard.IsNullOrWhiteSpace(settings?.ConnectionString, nameof(settings.ConnectionString));
            
            _settings = settings;
        }
        
        public IDbConnection CreateConnection() => new SqlConnection(_settings.ConnectionString);
    }
}