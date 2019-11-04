using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.Models;
using Microsoft.Azure.Cosmos;

namespace CalculateFunding.Common.CosmosDb
{
    public interface ICosmosRepository
    {
        (bool Ok, string Message) IsHealthOk();

        Task EnsureContainerExists();

        Task<ThroughputResponse> SetThroughput(int requestUnits);

        Task<int?> GetThroughput();

        IQueryable<DocumentEntity<T>> Read<T>(int itemsPerPage = 1000, bool enableCrossPartitionQuery = false) where T : IIdentifiable;

        DocumentEntity<T> ReadDocumentById<T>(string id, bool enableCrossPartitionQuery = false) where T : IIdentifiable;

        T ReadById<T>(string id) where T : IIdentifiable;

        Task<T> ReadByIdPartitionedAsync<T>(string id, string partitionKey) where T : IIdentifiable;

        /// <summary>
        /// Query cosmos using IQueryable on a given entity.
        /// </summary>
        /// <typeparam name="T">Type of document stored in cosmos</typeparam>
        /// <param name="enableCrossPartitionQuery">Enable cross partitioned query</param>
        /// <returns></returns>
        IQueryable<T> Query<T>(bool enableCrossPartitionQuery = false) where T : IIdentifiable;

        Task<IEnumerable<T>> QueryPartitionedEntity<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, string partitionKey = null) where T : IIdentifiable;

        Task<IEnumerable<T>> QuerySql<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, bool enableCrossPartitionQuery = false) where T : IIdentifiable;

        Task<IEnumerable<dynamic>> DynamicQuery(CosmosDbQuery cosmosDbQuery, bool enableCrossPartitionQuery = false);

        Task<IEnumerable<dynamic>> DynamicQuery(CosmosDbQuery cosmosDbQuery, bool enableCrossPartitionQuery = false, int itemsPerPage = 1000);

        Task<IEnumerable<dynamic>> DynamicQueryPartitionedEntity<dynamic>(CosmosDbQuery cosmosDbQuery, string partitionEntityId = null);

        Task<IEnumerable<T>> RawQuery<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, bool enableCrossPartitionQuery = false);

        Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(int itemsPerPage = 1000, Expression<Func<DocumentEntity<T>, bool>> query = null, bool enableCrossPartitionQuery = true) where T : IIdentifiable;

        Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = 1000, bool enableCrossPartitionQuery = true) where T : IIdentifiable;

        Task DocumentsBatchProcessingAsync<T>(Func<List<DocumentEntity<T>>, Task> persistBatchToIndex, int itemsPerPage = 1000, Expression<Func<DocumentEntity<T>, bool>> query = null) where T : IIdentifiable;

        Task DocumentsBatchProcessingAsync<T>(Func<List<T>, Task> persistBatchToIndex, CosmosDbQuery cosmosDbQuery, int itemsPerPage = 1000) where T : IIdentifiable;

        IQueryable<DocumentEntity<T>> QueryDocuments<T>(int itemsPerPage = -1) where T : IIdentifiable;

        IEnumerable<string> QueryAsJson(int itemsPerPage = -1);

        Task<IEnumerable<string>> QueryAsJsonAsync(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1);

        Task<HttpStatusCode> DeleteAsync<T>(string id, string partitionKey, bool enableCrossPartitionQuery = false, bool hardDelete = false) where T : IIdentifiable;

        Task<HttpStatusCode> CreateAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable;

        Task<DocumentEntity<T>> CreateDocumentAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable;

        Task<HttpStatusCode> UpsertAsync<T>(T entity, string partitionKey = null, bool enableCrossPartitionQuery = false, bool undelete = false, bool maintainCreatedDate = true) where T : IIdentifiable;

        Task<HttpStatusCode> CreateAsync<T>(KeyValuePair<string, T> entity) where T : IIdentifiable;

        Task<ItemResponse<DocumentEntity<T>>> CreateWithResponseAsync<T>(T entity) where T : IIdentifiable;

        Task BulkCreateAsync<T>(IList<T> entities, int degreeOfParallelism = 5) where T : IIdentifiable;

        Task BulkCreateAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5) where T : IIdentifiable;

        Task BulkDeleteAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5, bool hardDelete = false) where T : IIdentifiable;

        Task BulkUpsertAsync<T>(IList<T> entities, int degreeOfParallelism = 5, bool enableCrossPartitionQuery = false, bool maintainCreatedDate = true, bool undelete = false) where T : IIdentifiable;

        Task BulkUpsertAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5, bool enableCrossPartitionQuery = false, bool maintainCreatedDate = true, bool undelete = false) where T : IIdentifiable;

        Task<HttpStatusCode> UpdateAsync<T>(T entity, bool undelete = false) where T : Reference;

        Task<HttpStatusCode> BulkUpdateAsync<T>(IEnumerable<T> entities, string storedProcedureName) where T : IIdentifiable;
    }
}