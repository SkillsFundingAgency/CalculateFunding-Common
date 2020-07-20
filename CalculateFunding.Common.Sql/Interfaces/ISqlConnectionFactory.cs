using System.Data;

namespace CalculateFunding.Common.Sql.Interfaces
{
    public interface ISqlConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}