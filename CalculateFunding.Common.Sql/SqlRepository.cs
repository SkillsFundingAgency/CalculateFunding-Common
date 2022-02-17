using CalculateFunding.Common.Sql.Interfaces;
using CalculateFunding.Common.Utility;
using Dapper;
using Dapper.Contrib.Extensions;
using Polly;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Sql
{
    public abstract class SqlRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;
        private readonly AsyncPolicy _queryAsyncPolicy;
        private readonly Policy _openConnectionPolicy;
        private readonly Policy _executePolicy;

        protected SqlRepository(ISqlConnectionFactory connectionFactory,
            ISqlPolicyFactory sqlPolicyFactory)
        {
            Guard.ArgumentNotNull(connectionFactory, nameof(connectionFactory));
            Guard.ArgumentNotNull(sqlPolicyFactory, nameof(sqlPolicyFactory));

            _connectionFactory = connectionFactory;
            _queryAsyncPolicy = sqlPolicyFactory.CreateQueryAsyncPolicy();
            _openConnectionPolicy = sqlPolicyFactory.CreateConnectionOpenPolicy();
            _executePolicy = sqlPolicyFactory.CreateExecutePolicy();
        }

        public Task<(bool Ok, string Message)> IsHealthOk()
        {
            try
            {
                using IDbConnection connection = NewOpenConnection();

                return Task.FromResult((true, "SqlRepository is able to connect to the configured azure sql"));
            }
            catch (Exception ex)
            {
                return Task.FromResult((false, ex.Message));
            }
        }

        protected async Task<TEntity> QuerySingle<TEntity>(string sql,
            object parameters = null,
            ISqlTransaction transaction = null) =>
            await QuerySingle<TEntity>(sql,
                CommandType.StoredProcedure,
                parameters,
                transaction);

        protected async Task<TEntity> QuerySingleSql<TEntity>(string sql,
            object parameters = null,
            ISqlTransaction transaction = null) =>
            await QuerySingle<TEntity>(sql,
                CommandType.Text,
                parameters,
                transaction);

        protected async Task<int> Insert<TEntity>(TEntity entity, ISqlTransaction transaction = null) where TEntity : class
        {
            if (transaction == null)
            {
                using IDbConnection connection = NewOpenConnection();

                return await connection.InsertAsync(entity);
            }
            else
            {
                SqlTransaction sqlTransaction = transaction as SqlTransaction;
                return await sqlTransaction.InternalConnection.InsertAsync(entity, sqlTransaction.InternalTransaction);
            }
        }

        protected async Task<bool> Update<TEntity>(TEntity entity, ISqlTransaction transaction = null) where TEntity : class
        {
            if (transaction == null)
            {
                using IDbConnection connection = NewOpenConnection();

                return await connection.UpdateAsync(entity);
            }
            else
            {
                SqlTransaction sqlTransaction = transaction as SqlTransaction;
                return await sqlTransaction.InternalConnection.UpdateAsync(entity, sqlTransaction.InternalTransaction);
            }
        }

        protected async Task<bool> Delete<TEntity>(TEntity entity, ISqlTransaction transaction = null) where TEntity : class
        {
            if (transaction == null)
            {
                using IDbConnection connection = NewOpenConnection();

                return await connection.DeleteAsync(entity);
            }
            else
            {
                SqlTransaction sqlTransaction = transaction as SqlTransaction;
                return await sqlTransaction.InternalConnection.DeleteAsync(entity, sqlTransaction.InternalTransaction);
            }
        }

        private async Task<TEntity> QuerySingle<TEntity>(string sql,
            CommandType commandType,
            object parameters,
            ISqlTransaction transaction)
        {
            if (transaction == null)
            {
                using IDbConnection connection = NewOpenConnection();
                return await QuerySingleInternal<TEntity>(sql, commandType, parameters, connection);
            }
            else
            {
                SqlTransaction sqlTransaction = transaction as SqlTransaction;
                return await QuerySingleInternal<TEntity>(sql, commandType, parameters, sqlTransaction.InternalConnection, sqlTransaction.InternalTransaction);
            }

            async Task<TEntity> QuerySingleInternal<TEntityInternal>(string sql, CommandType commandType, object parameters, IDbConnection connection, IDbTransaction dbTransaction = null)
            {
                return await _queryAsyncPolicy.ExecuteAsync(() => connection.QuerySingleOrDefaultAsync<TEntity>(sql,
                                    parameters ?? new { },
                                    commandType: commandType,
                                    transaction: dbTransaction));
            }
        }

        protected async Task<IEnumerable<TEntity>> Query<TEntity>(string sql,
            object parameters = null,
            ISqlTransaction transaction = null) =>
            await Query<TEntity>(sql,
                CommandType.StoredProcedure,
                parameters,
                transaction);

        protected async Task<IEnumerable<TEntity>> QuerySql<TEntity>(string sql,
            object parameters = null,
            ISqlTransaction transaction = null) =>
            await Query<TEntity>(sql,
                CommandType.Text,
                parameters,
                transaction);

        protected int ExecuteNonQuery(string sql, ISqlTransaction transaction = null)
        {
            if (transaction == null)
            {
                using IDbConnection connection = NewOpenConnection();
                return ExecuteInternal(sql, connection);
            }
            else
            {
                SqlTransaction sqlTransaction = transaction as SqlTransaction;
                return ExecuteInternal(sql, sqlTransaction.InternalConnection);
            }

            int ExecuteInternal(string sql, IDbConnection connection, IDbTransaction dbTransaction = null)
            {
                IDbCommand command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                command.CommandTimeout = 120;

                // ReSharper disable once AccessToDisposedClosure
                return _executePolicy.Execute(() => command.ExecuteNonQuery());
            }
        }

        private async Task<IEnumerable<TEntity>> Query<TEntity>(string sql,
            CommandType commandType,
            object parameters,
            ISqlTransaction transaction)
        {

            if (transaction == null)
            {
                using IDbConnection connection = NewOpenConnection();

                return await QueryInternal<TEntity>(sql, commandType, parameters, connection);
            }
            else
            {
                SqlTransaction sqlTransaction = transaction as SqlTransaction;

                return await QueryInternal<TEntity>(sql, commandType, parameters, sqlTransaction.InternalConnection, sqlTransaction.InternalTransaction);
            }

            async Task<IEnumerable<TEntity>> QueryInternal<TEntityInternal>(string sql, CommandType commandType, object parameters, IDbConnection connection, IDbTransaction dbTransaction = null)
            {
                return (await _queryAsyncPolicy.ExecuteAsync(() => connection.QueryAsync<TEntity>(sql,
                        parameters ?? new { },
                        commandType: commandType,
                        transaction: dbTransaction)))
                    .ToArray();
            }
        }

        private IDbConnection NewOpenConnection()
        {
            IDbConnection connection = _connectionFactory.CreateConnection();

            _openConnectionPolicy.Execute(() => connection.Open());

            return connection;
        }
        protected ISqlTransaction BeginTransaction()
        {
            return new SqlTransaction(NewOpenConnection());
        }
    }
}