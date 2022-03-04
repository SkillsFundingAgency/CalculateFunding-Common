using CalculateFunding.Common.Sql.Interfaces;
using System.Data;

namespace CalculateFunding.Common.Sql
{
    public class SqlTransaction : ISqlTransaction 
    {
        private readonly IDbConnection _connection;

        private IDbTransaction _transaction;

        internal virtual IDbConnection InternalConnection => _connection;

        public virtual IDbTransaction CurrentTransaction => _transaction ??= _connection.BeginTransaction();

        internal SqlTransaction(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Commit()
        {
            CurrentTransaction.Commit();
        }

        public void Rollback()
        {
            CurrentTransaction.Rollback();
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
