using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CalculateFunding.Common.CosmosDb
{
    public interface ICosmosRepository
    {
        Task<(bool Ok, string Message)> IsHealthOk();

        Task EnsureCollectionExists();

        Task SetThroughput(int requestUnits);

        IQueryable<DocumentEntity<T>> Read<T>(int itemsPerPage = 1000) where T : IIdentifiable;

        Task<DocumentEntity<T>> ReadAsync<T>(string id) where T : IIdentifiable;

        /// <summary>
        /// Query cosmos using IQueryable on a given entity.
        /// </summary>
        /// <typeparam name="T">Type of document stored in cosmos</typeparam>
        /// <param name="enableCrossPartitionQuery">Enable cross partitioned query</param>
        /// <returns></returns>
        IQueryable<T> Query<T>(bool enableCrossPartitionQuery = false) where T : IIdentifiable;

        /// <summary>
        /// Query cosmos using IQueryable on a given entity.
        /// NOTE: The directSql may not work, only linq queries
        /// </summary>
        /// <typeparam name="T">Type of document stored in cosmos</typeparam>
        /// <param name="directSql">Direct SQL Query - may not work</param>
        /// <param name="enableCrossPartitionQuery">Enable cross partitioned query</param>
        /// <returns></returns>
        [Obsolete]
        IQueryable<T> Query<T>(string directSql, bool enableCrossPartitionQuery = false) where T : IIdentifiable;

        /// <summary>
        /// Query cosmos using IQueryable on a given entity.
        /// NOTE: The directSql may not work, only linq queries
        /// </summary>
        /// <typeparam name="T">Type of document stored in cosmos</typeparam>
        /// <param name="sqlQuerySpec">SQL Query Spec - may not work</param>
        /// <param name="enableCrossPartitionQuery">Enable cross partitioned query</param>
        /// <returns></returns>
        IQueryable<T> Query<T>(SqlQuerySpec sqlQuerySpec, bool enableCrossPartitionQuery = false) where T : IIdentifiable;

        [Obsolete]
        Task<IEnumerable<T>> QueryPartitionedEntity<T>(string directSql, int itemsPerPage = -1, string partitionEntityId = null) where T : IIdentifiable;

        Task<IEnumerable<T>> QueryPartitionedEntity<T>(SqlQuerySpec sqlQuerySpec, int itemsPerPage = -1, string partitionEntityId = null) where T : IIdentifiable;

        [Obsolete]
        IQueryable<dynamic> DynamicQuery<dynamic>(string sql, bool enableCrossPartitionQuery = false);

        IQueryable<dynamic> DynamicQuery<dynamic>(SqlQuerySpec sqlQuerySpec, bool enableCrossPartitionQuery = false);

        [Obsolete]
        IQueryable<dynamic> DynamicQueryPartionedEntity<dynamic>(string sql, string partitionEntityId = null);

        IQueryable<dynamic> DynamicQueryPartionedEntity<dynamic>(SqlQuerySpec sqlQuerySpec, string partitionEntityId = null);

        [Obsolete]
        Task<IEnumerable<dynamic>> QueryDynamic(string sql, bool enableCrossPartitionQuery = false, int itemsPerPage = 1000);

        Task<IEnumerable<dynamic>> QueryDynamic(SqlQuerySpec sqlQuerySpec, bool enableCrossPartitionQuery = false, int itemsPerPage = 1000);

        [Obsolete]
        IQueryable<T> RawQuery<T>(string directSql, int itemsPerPage = -1, bool enableCrossPartitionQuery = false);

        IQueryable<T> RawQuery<T>(SqlQuerySpec sqlQuerySpec, int itemsPerPage = -1, bool enableCrossPartitionQuery = false);

        [Obsolete]
        Task<IEnumerable<T>> QuerySql<T>(string directSql, int itemsPerPage = -1, bool enableCrossPartitionQuery = false) where T : IIdentifiable;

        Task<IEnumerable<T>> QuerySql<T>(SqlQuerySpec sqlQuerySpec, int itemsPerPage = -1, bool enableCrossPartitionQuery = false) where T : IIdentifiable;

        Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(int itemsPerPage = 1000, Expression<Func<DocumentEntity<T>, bool>> query = null, bool enableCrossPartitionQuery = true) where T : IIdentifiable;

        [Obsolete]
        Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(string sql, int itemsPerPage = 1000, bool enableCrossPartitionQuery = true) where T : IIdentifiable;

        Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(SqlQuerySpec sqlQuerySpec, int itemsPerPage = 1000, bool enableCrossPartitionQuery = true) where T : IIdentifiable;

        Task DocumentsBatchProcessingAsync<T>(Func<List<DocumentEntity<T>>, Task> persistBatchToIndex, int itemsPerPage = 1000, Expression<Func<DocumentEntity<T>, bool>> query = null) where T : IIdentifiable;

        [Obsolete]
        Task DocumentsBatchProcessingAsync<T>(Func<List<T>, Task> persistBatchToIndex, string sql, int itemsPerPage = 1000) where T : IIdentifiable;

        Task DocumentsBatchProcessingAsync<T>(Func<List<T>, Task> persistBatchToIndex, SqlQuerySpec sqlQuerySpec, int itemsPerPage = 1000) where T : IIdentifiable;

        IQueryable<DocumentEntity<T>> QueryDocuments<T>(string directSql = null, int itemsPerPage = -1) where T : IIdentifiable;

        IQueryable<DocumentEntity<T>> QueryDocuments<T>(SqlQuerySpec sqlQuerySpec, int itemsPerPage = -1) where T : IIdentifiable;

        IEnumerable<string> QueryAsJson(string directSql = null, int itemsPerPage = -1);

        IEnumerable<string> QueryAsJson(SqlQuerySpec sqlQuerySpec = null, int itemsPerPage = -1);

        Task<HttpStatusCode> DeleteAsync<T>(string id) where T : IIdentifiable;

        Task<HttpStatusCode> CreateAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable;

        Task<DocumentEntity<T>> CreateDocumentAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable;

        Task<HttpStatusCode> UpsertAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable;

        Task<HttpStatusCode> CreateAsync<T>(KeyValuePair<string, T> entity) where T : IIdentifiable;

        Task<ResourceResponse<Document>> CreateWithResponseAsync<T>(T entity) where T : IIdentifiable;

        Task BulkCreateAsync<T>(IList<T> entities, int degreeOfParallelism = 5) where T : IIdentifiable;

        Task BulkCreateAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5) where T : IIdentifiable;

        Task BulkUpsertAsync<T>(IList<T> entities, int degreeOfParallelism = 5) where T : IIdentifiable;

        Task BulkUpsertAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5) where T : IIdentifiable;

        Task<HttpStatusCode> UpdateAsync<T>(T entity) where T : Reference;

        Task<HttpStatusCode> BulkUpdateAsync<T>(IEnumerable<T> entities, string storedProcedureName) where T : IIdentifiable;

        int GetThroughput();
    }
}
