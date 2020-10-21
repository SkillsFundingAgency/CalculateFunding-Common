using System;
using System.Collections.Generic;
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

        Task<IEnumerable<DocumentEntity<T>>> Read<T>(int itemsPerPage = 1000) where T : IIdentifiable;

        Task<DocumentEntity<T>> ReadDocumentByIdAsync<T>(string id) where T : IIdentifiable;

        Task<DocumentEntity<T>> ReadDocumentByIdPartitionedAsync<T>(string id, string partitionKey) where T : IIdentifiable;

        /// <summary>
        /// Read item by ID for partition and don't throw if not found
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="id">Cosmos item ID</param>
        /// <param name="partitionKey">Partition key</param>
        /// <returns>Document or default(DocumentEntity<T>) if not found</returns>
        Task<DocumentEntity<T>> TryReadDocumentByIdPartitionedAsync<T>(string id, string partitionKey) where T : IIdentifiable;

        Task<T> ReadByIdAsync<T>(string id) where T : IIdentifiable;

        Task<T> ReadByIdPartitionedAsync<T>(string id, string partitionKey) where T : IIdentifiable;

        /// <summary>
        /// Read item by ID for partition and don't throw if not found
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="id">Cosmos item ID</param>
        /// <param name="partitionKey">Partition key</param>
        /// <returns>Document or default(T) if not found</returns>
        Task<T> TryReadByIdPartitionedAsync<T>(string id, string partitionKey) where T : IIdentifiable;

        /// <summary>
        /// Query cosmos using IQueryable on a given entity.
        /// </summary>
        /// <typeparam name="T">Type of document stored in cosmos</typeparam>
        /// <returns></returns>
        Task<IEnumerable<T>> Query<T>(Expression<Func<DocumentEntity<T>, bool>> query = null, int itemsPerPage = -1, int? maxItemCount = null) where T : IIdentifiable;

        Task<IEnumerable<T>> QueryPartitionedEntity<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, int? maxItemCount = null, string partitionKey = null) where T : IIdentifiable;

        Task<IEnumerable<T>> QuerySql<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, int? maxItemCount = null) where T : IIdentifiable;

        Task<IEnumerable<dynamic>> DynamicQuery(CosmosDbQuery cosmosDbQuery);

        Task<IEnumerable<dynamic>> DynamicQuery(CosmosDbQuery cosmosDbQuery, int itemsPerPage = 1000);

        Task<IEnumerable<dynamic>> DynamicQueryPartitionedEntity<dynamic>(CosmosDbQuery cosmosDbQuery, string partitionEntityId = null);

        Task<IEnumerable<T>> RawQuery<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, int? maxItemCount = null);

        Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(int itemsPerPage = 1000, Expression<Func<DocumentEntity<T>, bool>> query = null) where T : IIdentifiable;

        Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = 1000) where T : IIdentifiable;

        Task DocumentsBatchProcessingAsync<T>(Func<List<DocumentEntity<T>>, Task> persistBatchToIndex, int itemsPerPage = 1000, Expression<Func<DocumentEntity<T>, bool>> query = null) where T : IIdentifiable;

        Task DocumentsBatchProcessingAsync<T>(Func<List<T>, Task> persistBatchToIndex, CosmosDbQuery cosmosDbQuery, int itemsPerPage = 1000) where T : IIdentifiable;

        Task<IEnumerable<DocumentEntity<T>>> QueryDocuments<T>(int itemsPerPage = -1, int? maxItemCount = null) where T : IIdentifiable;

        Task<IEnumerable<string>> QueryAsJson(int itemsPerPage = -1, int? maxItemCount = null);

        Task<IEnumerable<string>> QueryAsJsonAsync(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, int? maxItemCount = null);

        Task<HttpStatusCode> DeleteAsync<T>(string id, string partitionKey, bool hardDelete = false) where T : IIdentifiable;

        Task<HttpStatusCode> CreateAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable;

        Task<DocumentEntity<T>> CreateDocumentAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable;

        Task<HttpStatusCode> UpsertAsync<T>(T entity, string partitionKey = null, bool undelete = false, bool maintainCreatedDate = true) where T : IIdentifiable;

        Task<HttpStatusCode> CreateAsync<T>(KeyValuePair<string, T> entity) where T : IIdentifiable;

        Task<ItemResponse<DocumentEntity<T>>> CreateWithResponseAsync<T>(T entity) where T : IIdentifiable;

        Task BulkCreateAsync<T>(IList<T> entities, int degreeOfParallelism = 5) where T : IIdentifiable;

        Task BulkCreateAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5) where T : IIdentifiable;

        Task BulkDeleteAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5, bool hardDelete = false) where T : IIdentifiable;

        Task BulkUpsertAsync<T>(IList<T> entities, int degreeOfParallelism = 5, bool maintainCreatedDate = true, bool undelete = false) where T : IIdentifiable;

        Task BulkUpsertAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5, bool maintainCreatedDate = true, bool undelete = false) where T : IIdentifiable;

        Task<HttpStatusCode> UpdateAsync<T>(T entity, bool undelete = false) where T : Reference;

        Task<HttpStatusCode> BulkUpdateAsync<T>(IEnumerable<T> entities, string storedProcedureName) where T : IIdentifiable;
        ICosmosDbFeedIterator<T> GetFeedIterator<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, int? maxItemCount = null) where T : IIdentifiable;
    }
}