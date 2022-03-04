using System;
using System.Data;

namespace CalculateFunding.Common.Sql.Interfaces
{
    public interface ISqlTransaction : IDisposable
    {
        void Commit();

        void Rollback();

        IDbTransaction CurrentTransaction { get; }
    }
}
