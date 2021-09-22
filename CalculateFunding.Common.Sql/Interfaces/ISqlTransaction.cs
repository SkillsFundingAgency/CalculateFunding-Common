using System;

namespace CalculateFunding.Common.Sql.Interfaces
{
    public interface ISqlTransaction : IDisposable
    {
        void Commit();

        void Rollback();
    }
}
