using System.Data;

namespace CalculateFunding.Common.Sql.UnitTests
{
    public static class DataTableExtensions
    {
        internal static DataTableReader ToDataTableReader(this DataTable dataTable) =>
            new DataTableReader(dataTable);
    }
}