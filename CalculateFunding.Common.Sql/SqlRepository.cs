using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.Sql.Interfaces;
using CalculateFunding.Common.Utility;
using Dapper;
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

        private IDbConnection NewOpenConnection()
        {
            IDbConnection connection = _connectionFactory.CreateConnection();

            _openConnectionPolicy.Execute(() => connection.Open());

            return connection;
        }
    }
}