using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.Helpers;
using CalculateFunding.Common.Sql.Interfaces;
using CalculateFunding.Common.Utility;
using Dapper;
using Dapper.Contrib.Extensions;
using Polly;

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
            object parameters = null) =>
            await QuerySingle<TEntity>(sql,
                CommandType.StoredProcedure,
                parameters);

        protected async Task<TEntity> QuerySingleSql<TEntity>(string sql,
            object parameters = null) =>
            await QuerySingle<TEntity>(sql,
                CommandType.Text,
                parameters);

        protected async Task<bool> BulkDelete<TEntity>(IList<TEntity> entities,
            int degreeOfParallelism = 5) where TEntity : class
        {
            return await BulkOperation((entity, connection, transaction) =>
                connection.DeleteAsync(entity, transaction),
                entities,
                degreeOfParallelism);
        }

        protected async Task<bool> BulkDelete<TEntity>(IList<TEntity> entities,
            IDbConnection connection,
            IDbTransaction transaction,
            int degreeOfParallelism = 5) where TEntity : class
        {
            return await BulkOperation((entity, connection, transaction) =>
                connection.DeleteAsync(entity, transaction),
                connection,
                transaction,
                entities,
                degreeOfParallelism);
        }

        protected async Task<bool> BulkUpdate<TEntity>(IList<TEntity> entities,
            int degreeOfParallelism = 5) where TEntity : class
        {
            return await BulkOperation((entity, connection, transaction) => 
                connection.UpdateAsync(entity, transaction),
                entities,
                degreeOfParallelism);
        }

        protected async Task<bool> BulkUpdate<TEntity>(IList<TEntity> entities, 
            IDbConnection connection, 
            IDbTransaction transaction, 
            int degreeOfParallelism = 5) where TEntity : class
        {
            return await BulkOperation((entity, connection, transaction) => 
                connection.UpdateAsync(entity, transaction), 
                connection, 
                transaction, 
                entities, 
                degreeOfParallelism);
        }

        protected async Task<bool> BulkInsert<TEntity>(IList<TEntity> entities, 
            int degreeOfParallelism = 5) where TEntity : class
        {
            return await BulkOperation(async(entity, connection, transaction) => 
                (await connection.InsertAsync(entity, transaction)) > 0,
                entities,
                degreeOfParallelism);
        }

        protected async Task<bool> BulkInsert<TEntity>(IList<TEntity> entities, 
            IDbConnection connection, 
            IDbTransaction transaction, 
            int degreeOfParallelism = 5) where TEntity : class
        {
            return await BulkOperation(async(entity, connection, transaction) =>
                (await connection.InsertAsync(entity, transaction)) > 0,
                connection,
                transaction,
                entities,
                degreeOfParallelism);
        }

        private async Task<bool> BulkOperation<TEntity>(Func<TEntity, IDbConnection, IDbTransaction, Task<bool>> action, 
            IList<TEntity> entities, 
            int degreeOfParallelism) where TEntity : class
        {
            using IDbConnection connection = NewOpenConnection();

            using IDbTransaction transaction = BeginTransaction(connection);

            try
            {
                bool success = await BulkOperation(action, connection, transaction, entities, degreeOfParallelism);

                if (success)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }

                return success;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task<bool> BulkOperation<TEntity>(Func<TEntity, IDbConnection, IDbTransaction, Task<bool>> action, 
            IDbConnection connection, 
            IDbTransaction transaction, 
            IList<TEntity> entities, 
            int degreeOfParallelism) where TEntity : class
        {
            List<Task<bool>> allTasks = new List<Task<bool>>(entities.Count);
            SemaphoreSlim throttler = new SemaphoreSlim(degreeOfParallelism);

            foreach (TEntity entity in entities)
            {
                await throttler.WaitAsync();

                allTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            return await action(entity, connection, transaction);
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
            }

            await TaskHelper.WhenAllAndThrow(allTasks.ToArray());

            return allTasks.Select(_ => _.Result).All(_ => _ == true);
        }

        protected IDbTransaction BeginTransaction(IDbConnection connection)
        {
            return connection.BeginTransaction();
        }

        protected void CommitTransaction(IDbTransaction transaction)
        {
            transaction.Commit();
        }

        protected async Task<int> Insert<TEntity>(TEntity entity) where TEntity : class
        {
            using IDbConnection connection = NewOpenConnection();

            return await connection.InsertAsync(entity);
        }

        protected async Task<bool> Update<TEntity>(TEntity entity) where TEntity : class
        {
            using IDbConnection connection = NewOpenConnection();

            return await connection.UpdateAsync(entity);
        }

        protected async Task<bool> Delete<TEntity>(TEntity entity) where TEntity : class
        {
            using IDbConnection connection = NewOpenConnection();

            return await connection.DeleteAsync(entity);
        }

        private async Task<TEntity> QuerySingle<TEntity>(string sql,
            CommandType commandType,
            object parameters)
        {
            using IDbConnection connection = NewOpenConnection();

            return await _queryAsyncPolicy.ExecuteAsync(() => connection.QuerySingleOrDefaultAsync<TEntity>(sql,
                parameters ?? new
                {
                },
                commandType: commandType));
        }

        protected async Task<IEnumerable<TEntity>> Query<TEntity>(string sql,
            object parameters = null) =>
            await Query<TEntity>(sql,
                CommandType.StoredProcedure,
                parameters);

        protected async Task<IEnumerable<TEntity>> QuerySql<TEntity>(string sql,
            object parameters = null) =>
            await Query<TEntity>(sql,
                CommandType.Text,
                parameters);

        protected int ExecuteNoneQuery(string sql)
        {
            using IDbConnection connection = NewOpenConnection();
            using IDbCommand command = connection.CreateCommand();

            command.CommandText = sql;
            command.CommandType = CommandType.Text;

            // ReSharper disable once AccessToDisposedClosure
            return _executePolicy.Execute(() => command.ExecuteNonQuery());
        }

        private async Task<IEnumerable<TEntity>> Query<TEntity>(string sql,
            CommandType commandType,
            object parameters)
        {
            using IDbConnection connection = NewOpenConnection();

            return (await _queryAsyncPolicy.ExecuteAsync(() => connection.QueryAsync<TEntity>(sql,
                    parameters ?? new
                    {
                    },
                    commandType: commandType)))
                .ToArray();
        }

        protected IDbConnection NewOpenConnection()
        {
            IDbConnection connection = _connectionFactory.CreateConnection();

            _openConnectionPolicy.Execute(() => connection.Open());

            return connection;
        }
    }
}