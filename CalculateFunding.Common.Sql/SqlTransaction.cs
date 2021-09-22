using CalculateFunding.Common.Sql.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace CalculateFunding.Common.Sql
{
    public class SqlTransaction : ISqlTransaction 
    {
        private readonly IDbConnection _connection;

        private IDbTransaction _transaction;

        internal virtual IDbConnection InternalConnection => _connection;

        internal virtual IDbTransaction InternalTransaction => _transaction ??= _connection.BeginTransaction();

        internal SqlTransaction(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Commit()
        {
            InternalTransaction.Commit();
        }

        public void Rollback()
        {
            InternalTransaction.Rollback();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _connection?.Dispose();
            }
        }
    }
}
