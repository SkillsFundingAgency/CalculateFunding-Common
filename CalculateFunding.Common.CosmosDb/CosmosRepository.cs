using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.Models;
using CalculateFunding.Common.Utility;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Scripts;
using Newtonsoft.Json;

namespace CalculateFunding.Common.CosmosDb
{
    public class CosmosRepository : ICosmosRepository, IDisposable
    {
        private const int _defaultThroughput = 400;

        private readonly string _containerName;
        private readonly string _partitionKey;
        private readonly string _databaseName;
        private readonly CosmosClient _cosmosClient;
        private Database _database;
        private Container _container;

        public CosmosRepository(CosmosDbSettings settings)
        {
            Guard.ArgumentNotNull(settings, nameof(settings));
            Guard.IsNullOrWhiteSpace(settings.ContainerName, nameof(settings.ContainerName));
            Guard.IsNullOrWhiteSpace(settings.ConnectionString, nameof(settings.ConnectionString));
            Guard.IsNullOrWhiteSpace(settings.DatabaseName, nameof(settings.DatabaseName));

            _containerName = settings.ContainerName;
            _partitionKey = settings.PartitionKey;
            _databaseName = settings.DatabaseName;
            _cosmosClient = CosmosDbConnectionString.Parse(settings.ConnectionString);

            _database = _cosmosClient.GetDatabase(_databaseName);

            if (_database != null)
            {
                _container = _database.GetContainer(_containerName);
            }
        }

        private QueryRequestOptions GetQueryRequestOptions(int itemsPerPage)
        {
            QueryRequestOptions queryRequestOptions = new QueryRequestOptions
            {
                MaxItemCount = itemsPerPage
            };

            return queryRequestOptions;
        }

        private QueryRequestOptions GetDefaultQueryRequestOptions(int? itemsPerPage = null,
            int? maxBufferedItemCount = null,
            int? maxConcurrency = null)
        {
            QueryRequestOptions queryRequestOptions = new QueryRequestOptions
            {
                MaxItemCount = itemsPerPage,
                MaxBufferedItemCount = maxBufferedItemCount ?? 100,
                MaxConcurrency = maxConcurrency ?? 50
            };

            return queryRequestOptions;
        }

        private QueryRequestOptions GetDefaultQueryRequestOptions(string partitionKey)
        {
            return new QueryRequestOptions
            {
                PartitionKey = !string.IsNullOrWhiteSpace(partitionKey)
                    ? new PartitionKey(partitionKey)
                    : PartitionKey.None
            };
        }

        private async Task<IEnumerable<T>> ResultsFromQueryAndOptions<T>(CosmosDbQuery cosmosDbQuery, QueryRequestOptions queryOptions)
        {
            FeedIterator<T> query = _container.GetItemQueryIterator<T>(
                queryDefinition: cosmosDbQuery.CosmosQueryDefinition,
                requestOptions: queryOptions);

            return await ResultsFromFeedIterator(query);
        }

        private async Task<IEnumerable<T>> ResultsFromQueryAndOptions<T>(CosmosDbQuery cosmosDbQuery, Func<List<T>, Task> batchAction, QueryRequestOptions queryOptions)
        {
            FeedIterator<T> query = _container.GetItemQueryIterator<T>(
                queryDefinition: cosmosDbQuery.CosmosQueryDefinition,
                requestOptions: queryOptions);

            return await ResultsFromFeedIterator(query, batchAction, queryOptions.MaxItemCount ?? 0);
        }

        private async Task<IEnumerable<T>> ResultsFromFeedIterator<T>(FeedIterator<T> query)
        {
            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                foreach (T t in await query.ReadNextAsync())
                {
                    results.Add(t);
                }
            }

            return results;
        }

        private async Task<IEnumerable<T>> ResultsFromFeedIterator<T>(FeedIterator<T> query, Func<List<T>, Task> batchAction, int itemsPerPage)
        {
            List<T> results = new List<T>();
            while (query.HasMoreResults)
            {
                if (results.Count() >= itemsPerPage)
                {
                    await batchAction(results);
                    results.Clear();
                }

                results.AddRange(await query.ReadNextAsync());
            }

            if (results.Count() > 0)
            {
                await batchAction(results);
                results.Clear();
            }

            return results;
        }

        private IEnumerable<string> JsonFromDocuments(IEnumerable<Document> documents)
        {
            foreach (Document document in documents)
            {
                dynamic json = document;
                yield return JsonConvert.SerializeObject((object)json.Content); // haven't tried this yet!
            }
        }

        private async Task<ItemResponse<T>> HardDeleteAsync<T>(DocumentEntity<T> entity, string partitionKey) where T : IIdentifiable
        {
            Guard.IsNullOrWhiteSpace(partitionKey, nameof(partitionKey));

            ItemResponse<T> response = await _container.DeleteItemAsync<T>(id: entity.Id, partitionKey: new PartitionKey(partitionKey));

            return response;
        }

        private async Task<ItemResponse<T>> SoftDeleteAsync<T>(DocumentEntity<T> entity) where T : IIdentifiable
        {
            entity.Deleted = true;
            ItemResponse<T> response = await _container.ReplaceItemAsync(item: entity.Content, id: entity.Id);

            return response;
        }

        private async Task<HttpStatusCode> DeleteAsync<T>(DocumentEntity<T> entity, string partitionKey, bool hardDelete = false) where T : IIdentifiable
        {
            ItemResponse<T> response;

            if (hardDelete)
            {
                response = await HardDeleteAsync(entity, partitionKey);
            }
            else
            {
                response = await SoftDeleteAsync(entity);
            }

            return response.StatusCode;
        }

        private static string GetDocumentType<T>()
        {
            return typeof(T).Name;
        }

        public (bool Ok, string Message) IsHealthOk()
        {
            try
            {
                _cosmosClient.GetDatabase(_databaseName);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private async Task<Database> CreateDatabaseIfNotExists(string databaseName)
        {
            if (_database != null) return _database;

            DatabaseResponse databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseName);

            if (databaseResponse.StatusCode == HttpStatusCode.OK)
            {
                return databaseResponse.Database;
            }
            else
            {
                throw new Exception($"Database not created: {databaseResponse.StatusCode}");
            }
        }

        private async Task<Container> CreateContainerIfNotExists(string containerName, string partitionKey, int defaultThroughput)
        {
            ContainerResponse containerResponse = await _database.CreateContainerIfNotExistsAsync(
                id: containerName,
                partitionKeyPath: partitionKey,
                throughput: defaultThroughput);

            if (containerResponse.StatusCode == HttpStatusCode.OK)
            {
                return containerResponse.Container;
            }
            else
            {
                throw new Exception($"Container not created: {containerResponse.StatusCode}");
            }
        }

        public async Task EnsureContainerExists()
        {
            if (_container == null)
            {
                _database = await CreateDatabaseIfNotExists(_databaseName);

                _container = await CreateContainerIfNotExists(_containerName, _partitionKey, _defaultThroughput);
            }
        }

        public async Task<ThroughputResponse> SetThroughput(int requestUnits)
        {
            return await _container.ReplaceThroughputAsync(requestUnits);
        }

        public async Task<int?> GetThroughput()
        {
            return await _container.ReadThroughputAsync();
        }

        public async Task<IEnumerable<DocumentEntity<T>>> Read<T>(int itemsPerPage = 1000) where T : IIdentifiable
        {
            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(itemsPerPage);

            FeedIterator<DocumentEntity<T>> feedIterator = _container
                .GetItemLinqQueryable<DocumentEntity<T>>(requestOptions: queryRequestOptions)
                .Where(x => x.DocumentType == GetDocumentType<T>() && !x.Deleted)
                .ToFeedIterator();

            return await ResultsFromFeedIterator(feedIterator);
        }

        public async Task<DocumentEntity<T>> ReadDocumentByIdAsync<T>(string id) where T : IIdentifiable
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));

            CosmosDbQuery cosmosDbQuery = new CosmosDbQuery
            {
                QueryText = @"SELECT *
                            FROM    c 
                            WHERE   c.documentType = @documentType 
                                    AND c.id = @id",
                Parameters = new[]
                {
                    new CosmosDbQueryParameter("@documentType", GetDocumentType<T>()),
                    new CosmosDbQueryParameter("@id", id)
                }
            };

            FeedIterator<DocumentEntity<T>> feedIterator = _container
                .GetItemQueryIterator<DocumentEntity<T>>(cosmosDbQuery.CosmosQueryDefinition);

            return (await ResultsFromFeedIterator(feedIterator)).SingleOrDefault();
        }

        public async Task<T> ReadByIdAsync<T>(string id) where T : IIdentifiable
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));

            DocumentEntity<T> result = await ReadDocumentByIdAsync<T>(id);

            if (result != null)
            {
                return result.Content;
            }
            else
            {
                return default(T);
            }
        }

        public async Task<T> ReadByIdPartitionedAsync<T>(string id, string partitionKey) where T : IIdentifiable
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));
            Guard.IsNullOrWhiteSpace(partitionKey, nameof(partitionKey));

            ItemResponse<T> response = await _container.ReadItemAsync<T>(id: id, partitionKey: new PartitionKey(partitionKey));

            return response.Resource;
        }

        public async Task<DocumentEntity<T>> ReadDocumentByIdPartitionedAsync<T>(string id, string partitionKey) where T : IIdentifiable
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));
            Guard.IsNullOrWhiteSpace(partitionKey, nameof(partitionKey));

            ItemResponse<DocumentEntity<T>> response = await _container.ReadItemAsync<DocumentEntity<T>>(id: id, partitionKey: new PartitionKey(partitionKey));

            return response.Resource;
        }

        /// <summary>
        /// Query cosmos using IQueryable on a given entity.
        /// </summary>
        /// <typeparam name="T">Type of document stored in cosmos</typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> Query<T>(Expression<Func<DocumentEntity<T>, bool>> query = null, int itemsPerPage = -1) where T : IIdentifiable
        {
            QueryRequestOptions queryRequestOptions = GetDefaultQueryRequestOptions(itemsPerPage: itemsPerPage);

            Expression<Func<DocumentEntity<T>, bool>> expression = x => x.DocumentType == GetDocumentType<T>() && !x.Deleted;

            FeedIterator<DocumentEntity<T>> feedIterator;

            if (query != null)
            {
                feedIterator = _container.GetItemLinqQueryable<DocumentEntity<T>>(requestOptions: queryRequestOptions)
                    .Where(expression)
                    .Where(query)
                    .ToFeedIterator();
            }
            else
            {
                feedIterator = _container.GetItemLinqQueryable<DocumentEntity<T>>(requestOptions: queryRequestOptions)
                    .Where(expression)
                    .ToFeedIterator();
            }

            return (await ResultsFromFeedIterator(feedIterator)).Select(x => x.Content);
        }

        public async Task<IEnumerable<T>> QueryPartitionedEntity<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, string partitionKey = null) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryRequestOptions = GetDefaultQueryRequestOptions(itemsPerPage: itemsPerPage);
            queryRequestOptions.PartitionKey = new PartitionKey(partitionKey);

            IEnumerable<DocumentEntity<T>> documentResults = await ResultsFromQueryAndOptions<DocumentEntity<T>>(cosmosDbQuery, queryRequestOptions);

            return documentResults.Select(x => x.Content);
        }

        public async Task<IEnumerable<T>> QuerySql<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryOptions = GetQueryRequestOptions(itemsPerPage);

            return await ResultsFromQueryAndOptions<T>(cosmosDbQuery, queryOptions);
        }

        public async Task<IEnumerable<dynamic>> DynamicQuery(CosmosDbQuery cosmosDbQuery)
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryRequestOptions = GetDefaultQueryRequestOptions();

            return await ResultsFromQueryAndOptions<dynamic>(cosmosDbQuery, queryRequestOptions);
        }

        public async Task<IEnumerable<dynamic>> DynamicQuery(CosmosDbQuery cosmosDbQuery, int itemsPerPage = 1000)
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryRequestOptions = GetDefaultQueryRequestOptions(itemsPerPage: itemsPerPage);

            return await ResultsFromQueryAndOptions<dynamic>(cosmosDbQuery, queryRequestOptions);
        }

        public async Task<IEnumerable<dynamic>> DynamicQueryPartitionedEntity<dynamic>(CosmosDbQuery cosmosDbQuery, string partitionEntityId = null)
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryRequestOptions = GetDefaultQueryRequestOptions();
            queryRequestOptions.PartitionKey = new PartitionKey(partitionEntityId);

            return await ResultsFromQueryAndOptions<dynamic>(cosmosDbQuery, queryRequestOptions);
        }

        public async Task<IEnumerable<T>> RawQuery<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1)
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryOptions = GetDefaultQueryRequestOptions(itemsPerPage: itemsPerPage,
                maxBufferedItemCount: 50,
                maxConcurrency: 100);

            return await ResultsFromQueryAndOptions<T>(cosmosDbQuery, queryOptions);
        }

        public async Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(int itemsPerPage = 1000, Expression<Func<DocumentEntity<T>, bool>> query = null) where T : IIdentifiable
        {
            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(itemsPerPage);

            FeedIterator<DocumentEntity<T>> feedIterator;

            if (query == null)
            {
                feedIterator = _container
                    .GetItemLinqQueryable<DocumentEntity<T>>(requestOptions: queryRequestOptions)
                    .Where(d => d.DocumentType == GetDocumentType<T>())
                    .ToFeedIterator();
            }
            else
            {
                feedIterator = _container
                    .GetItemLinqQueryable<DocumentEntity<T>>(requestOptions: queryRequestOptions)
                    .Where(query)
                    .ToFeedIterator();
            }

            return await ResultsFromFeedIterator<DocumentEntity<T>>(feedIterator);
        }

        public async Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = 1000) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(itemsPerPage);

            FeedIterator<DocumentEntity<T>> documents = _container
                .GetItemQueryIterator<DocumentEntity<T>>(queryDefinition: cosmosDbQuery.CosmosQueryDefinition,
                    requestOptions: queryRequestOptions);

            return await ResultsFromFeedIterator(documents);
        }

        public async Task DocumentsBatchProcessingAsync<T>(Func<List<DocumentEntity<T>>, Task> persistBatchToIndex, int itemsPerPage = 1000, Expression<Func<DocumentEntity<T>, bool>> query = null) where T : IIdentifiable
        {
            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(itemsPerPage);

            IQueryable<DocumentEntity<T>> allResults;

            IOrderedQueryable<DocumentEntity<T>> queryable = _container.GetItemLinqQueryable<DocumentEntity<T>>(allowSynchronousQueryExecution: true, requestOptions: queryRequestOptions);

            if (query == null)
            {
                allResults = queryable.Where(d => d.DocumentType == GetDocumentType<T>());
            }
            else
            {
                allResults = queryable.Where(query);
            }

            await persistBatchToIndex(allResults.ToList());
        }

        public async Task DocumentsBatchProcessingAsync<T>(Func<List<T>, Task> persistBatchToIndex, CosmosDbQuery cosmosDbQuery, int itemsPerPage = 1000) where T : IIdentifiable
        {
            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(itemsPerPage);

            await ResultsFromQueryAndOptions(cosmosDbQuery, persistBatchToIndex, queryRequestOptions);
        }

        public async Task<IEnumerable<DocumentEntity<T>>> QueryDocuments<T>(int itemsPerPage = -1) where T : IIdentifiable
        {
            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(itemsPerPage);

            FeedIterator<DocumentEntity<T>> feedIterator = _container
                .GetItemLinqQueryable<DocumentEntity<T>>(requestOptions: queryRequestOptions)
                .ToFeedIterator();

            return await ResultsFromFeedIterator<DocumentEntity<T>>(feedIterator);
        }

        public async Task<IEnumerable<string>> QueryAsJson(int itemsPerPage = -1)
        {
            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(itemsPerPage);

            FeedIterator<Document> feedIterator = _container
                .GetItemLinqQueryable<Document>(requestOptions: queryRequestOptions)
                .ToFeedIterator();

            IEnumerable<Document> documents = await ResultsFromFeedIterator<Document>(feedIterator);

            return JsonFromDocuments(documents);
        }

        public async Task<IEnumerable<string>> QueryAsJsonAsync(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1)
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(itemsPerPage);

            IEnumerable<Document> documents = await ResultsFromQueryAndOptions<Document>(cosmosDbQuery, queryRequestOptions);

            return JsonFromDocuments(documents);
        }

        public async Task<HttpStatusCode> DeleteAsync<T>(string id, string partitionKey, bool hardDelete = false) where T : IIdentifiable
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));
            Guard.IsNullOrWhiteSpace(partitionKey, nameof(partitionKey));

            DocumentEntity<T> doc = await ReadDocumentByIdAsync<T>(id);

            return await DeleteAsync(doc, partitionKey, hardDelete);
        }

        private DocumentEntity<T> CreateDocumentEntity<T>(T entity) where T : IIdentifiable
        {
            DocumentEntity<T> document = CreateDocumentEntity<T>();
            document.Content = entity;

            return document;
        }

        private DocumentEntity<T> CreateDocumentEntity<T>() where T : IIdentifiable
        {
            return new DocumentEntity<T>()
            {
                DocumentType = GetDocumentType<T>(),
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            };
        }

        private async Task<ItemResponse<DocumentEntity<T>>> CreateDocumentInternalAsync<T>(T entity) where T : IIdentifiable
        {
            DocumentEntity<T> doc = CreateDocumentEntity(entity);

            return await _container.CreateItemAsync(doc);
        }

        public async Task<HttpStatusCode> CreateAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            ItemResponse<DocumentEntity<T>> response = await CreateDocumentInternalAsync(entity);

            return response.StatusCode;
        }

        public async Task<DocumentEntity<T>> CreateDocumentAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            ItemResponse<DocumentEntity<T>> response = await CreateDocumentInternalAsync(entity);

            return response.Resource;
        }

        public async Task<HttpStatusCode> UpsertAsync<T>(T entity, string partitionKey = null, bool undelete = false, bool maintainCreatedDate = true) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            QueryRequestOptions queryRequestOptions = GetDefaultQueryRequestOptions(partitionKey: partitionKey);

            DocumentEntity<T> doc = new DocumentEntity<T>();

            if (maintainCreatedDate)
            {
                //SingleOrDefault not supported on the current Cosmos driver
                List<DocumentEntity<T>> documents = _container
                    .GetItemLinqQueryable<DocumentEntity<T>>(allowSynchronousQueryExecution: true, requestOptions: queryRequestOptions)
                    .Where(d => d.Id == entity.Id)
                    .ToList();

                switch (documents.Count)
                {
                    case 0:
                        doc = CreateDocumentEntity<T>();
                        break;

                    case 1:
                        doc = documents.ElementAt(0);
                        doc.UpdatedAt = DateTimeOffset.Now;
                        break;

                    default:
                        throw new Exception($"Expected 1 record, found {documents.Count}, aborting");
                }

            }
            else
            {
                doc = CreateDocumentEntity<T>();
            }

            if (undelete)
            {
                // need to reset the deleted flag
                doc.Deleted = false;
            }

            doc.Content = entity;

            ItemResponse<DocumentEntity<T>> response = await _container.UpsertItemAsync(doc);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> CreateAsync<T>(KeyValuePair<string, T> entity) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entity, nameof(entity));
            Guard.ArgumentNotNull(entity.Value, nameof(entity.Value));

            DocumentEntity<T> doc = CreateDocumentEntity(entity.Value);

            ItemResponse<DocumentEntity<T>> response = await _container.CreateItemAsync(item: doc, partitionKey: new PartitionKey(entity.Key));
            return response.StatusCode;
        }

        public async Task<ItemResponse<DocumentEntity<T>>> CreateWithResponseAsync<T>(T entity) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            DocumentEntity<T> doc = CreateDocumentEntity(entity);

            return await _container.CreateItemAsync(doc);
        }

        public async Task BulkCreateAsync<T>(IList<T> entities, int degreeOfParallelism = 5) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entities, nameof(entities));

            await Task.Run(() => Parallel.ForEach(entities, new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism }, (item) =>
            {
                Task.WaitAll(CreateAsync(item));
            }));
        }

        public async Task BulkCreateAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entities, nameof(entities));

            List<Task> allTasks = new List<Task>(entities.Count());
            SemaphoreSlim throttler = new SemaphoreSlim(initialCount: degreeOfParallelism);

            foreach (KeyValuePair<string, T> entity in entities)
            {
                await throttler.WaitAsync();
                allTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            await CreateAsync(entity);
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
            }

            await Task.WhenAll(allTasks.ToArray());

            foreach (Task task in allTasks)
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
                }
            }
        }

        public async Task BulkDeleteAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5, bool hardDelete = false) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entities, nameof(entities));

            await Task.Run(() => Parallel.ForEach(entities, new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism }, (item) =>
            {
                DocumentEntity<T> document = new DocumentEntity<T>(item.Value)
                {
                    UpdatedAt = DateTimeOffset.Now
                };

                Task.WaitAll(DeleteAsync(entity: document, hardDelete: hardDelete, partitionKey: item.Key));
            }));
        }

        public async Task BulkUpsertAsync<T>(IList<T> entities, int degreeOfParallelism = 5, bool maintainCreatedDate = true, bool undelete = false) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entities, nameof(entities));

            await Task.Run(() => Parallel.ForEach(entities, new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism }, (item) =>
            {
                Task.WaitAll(UpsertAsync(item, item.Id, maintainCreatedDate: maintainCreatedDate, undelete: undelete));
            }));
        }

        public async Task BulkUpsertAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5, bool maintainCreatedDate = true, bool undelete = false) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entities, nameof(entities));

            List<Task> allTasks = new List<Task>(entities.Count());
            SemaphoreSlim throttler = new SemaphoreSlim(initialCount: degreeOfParallelism);
            foreach (KeyValuePair<string, T> entity in entities)
            {
                await throttler.WaitAsync();
                allTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            await UpsertAsync(entity: entity.Value, partitionKey: entity.Key, maintainCreatedDate: maintainCreatedDate, undelete: undelete);
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
            }
            await Task.WhenAll(allTasks.ToArray());

            foreach (Task task in allTasks)
            {
                if (task.Exception != null)
                {
                    throw task.Exception;
                }
            }
        }

        public async Task<HttpStatusCode> UpdateAsync<T>(T entity, bool undelete = false) where T : Reference
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            string documentType = GetDocumentType<T>();
            DocumentEntity<T> doc = new DocumentEntity<T>(entity);
            if (doc.DocumentType != null && doc.DocumentType != documentType)
            {
                throw new ArgumentException($"Cannot change {entity.Id} from {doc.DocumentType} to {typeof(T).Name}");
            }

            doc.DocumentType = documentType; // in case not specified
            doc.UpdatedAt = DateTimeOffset.Now;

            if (undelete)
            {
                // need to reset deleted flag
                doc.Deleted = false;
            }

            ItemResponse<DocumentEntity<T>> response = await _container.ReplaceItemAsync(item: doc, id: entity.Id);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> BulkUpdateAsync<T>(IEnumerable<T> entities, string storedProcedureName) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entities, nameof(entities));

            string documentType = GetDocumentType<T>();

            IList<DocumentEntity<T>> documents = new List<DocumentEntity<T>>();

            foreach (T entity in entities)
            {
                DocumentEntity<T> doc = new DocumentEntity<T>(entity);
                if (doc.DocumentType != null && doc.DocumentType != documentType)
                {
                    throw new ArgumentException($"Cannot change {entity.Id} from {doc.DocumentType} to {typeof(T).Name}");
                }

                doc.DocumentType = documentType;
                doc.UpdatedAt = DateTimeOffset.Now;
                documents.Add(doc);
            }

            string documentsAsJson = JsonConvert.SerializeObject(documents);

            dynamic[] args = new dynamic[] { JsonConvert.DeserializeObject<dynamic>(documentsAsJson) };

            Scripts cosmosScripts = _container.Scripts;

            StoredProcedureExecuteResponse<string> response = await cosmosScripts.ExecuteStoredProcedureAsync<string>(
                storedProcedureId: storedProcedureName,
                partitionKey: PartitionKey.Null,
                requestOptions: null,
                parameters: args);

            return response.StatusCode;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cosmosClient?.Dispose();
            }
        }
    }
}
