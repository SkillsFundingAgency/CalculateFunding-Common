using System;
using System.Data;

namespace CalculateFunding.Common.Sql.Interfaces
{
    public interface ISqlTransaction : IDisposable
    {
        void Commit();

        void Rollback();

        IDbConnection Connection { get; }

        IDbTransaction CurrentTransaction { get; }
    }
}
