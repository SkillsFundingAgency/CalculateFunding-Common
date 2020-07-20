using CalculateFunding.Common.Sql.Interfaces;

namespace CalculateFunding.Common.Sql
{
    public class SqlSettings : ISqlSettings
    {
        public string ConnectionString { get; set; }
    }
}