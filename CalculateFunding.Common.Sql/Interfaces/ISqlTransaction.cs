using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CalculateFunding.Common.Sql.Interfaces
{
    public interface ISqlTransaction : IDisposable
    {
        void Commit();

        void Rollback();
    }
}
