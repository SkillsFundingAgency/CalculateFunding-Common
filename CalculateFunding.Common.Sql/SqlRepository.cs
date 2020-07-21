using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.Sql.Interfaces;
using CalculateFunding.Common.Utility;
using Dapper;

namespace CalculateFunding.Common.Sql
{
    public abstract class SqlRepository
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        protected SqlRepository(ISqlConnectionFactory connectionFactory)
        {
            Guard.ArgumentNotNull(connectionFactory, nameof(connectionFactory));

            _connectionFactory = connectionFactory;
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

            return await connection.QuerySingleOrDefaultAsync(sql,
                parameters ?? new
                {
                },
                commandType: commandType);
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

        private async Task<IEnumerable<TEntity>> Query<TEntity>(string sql,
            CommandType commandType,
            object parameters)
        {
            using IDbConnection connection = NewOpenConnection();

            return (await connection.QueryAsync<TEntity>(sql,
                    parameters ?? new
                    {
                    },
                    commandType: commandType))
                .ToArray();
        }

        private IDbConnection NewOpenConnection()
        {
            IDbConnection connection = _connectionFactory.CreateConnection();

            connection.Open();

            return connection;
        }
    }
}