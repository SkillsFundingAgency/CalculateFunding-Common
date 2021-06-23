using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.Helpers;
using CalculateFunding.Common.Models;
using CalculateFunding.Common.Utility;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CalculateFunding.Common.CosmosDb
{
    public class CosmosRepository : ICosmosRepository, IDisposable
    {
        private const int _defaultThroughput = 400;
        private readonly ItemRequestOptions _disableContentResponseOnWriteRequestOptions = new ItemRequestOptions() { EnableContentResponseOnWrite = false };

        private readonly string _containerName;
        private readonly string _partitionKey;
        private readonly string _databaseName;
        private readonly string _connectionString;
        private readonly CosmosClientOptions _cosmosClientOptions;
        private CosmosClient _cosmosClient;
        private Database _database;
        private Container _container;

        public CosmosRepository(CosmosDbSettings settings, CosmosClientOptions cosmosClientOptions = null)
        {
            Guard.ArgumentNotNull(settings, nameof(settings));
            Guard.IsNullOrWhiteSpace(settings.ContainerName, nameof(settings.ContainerName));
            Guard.IsNullOrWhiteSpace(settings.ConnectionString, nameof(settings.ConnectionString));
            Guard.IsNullOrWhiteSpace(settings.DatabaseName, nameof(settings.DatabaseName));

            _containerName = settings.ContainerName;
            _partitionKey = settings.PartitionKey;
            _databaseName = settings.DatabaseName;
            _connectionString = settings.ConnectionString;
            _cosmosClientOptions = cosmosClientOptions;
        }

        private CosmosClient RepositoryClient =>_cosmosClient ??= GetClient(_connectionString, _cosmosClientOptions);

        private Database RepositoryDatabase => _database ??= RepositoryClient.GetDatabase(_databaseName);

        private Container RepositoryContainer => _container ??= RepositoryDatabase?.GetContainer(_containerName);

        private QueryRequestOptions GetQueryRequestOptions(int itemsPerPage)
        {
            return new QueryRequestOptions
            {
                MaxItemCount = itemsPerPage
            };
        }

        private QueryRequestOptions GetDefaultQueryRequestOptions(int? itemsPerPage = null,
            int? maxBufferedItemCount = null,
            int? maxConcurrency = null)
        {
            return new QueryRequestOptions
            {
                MaxItemCount = itemsPerPage == -1 ? 1000 : itemsPerPage,
                MaxBufferedItemCount = maxBufferedItemCount ?? 100,
                MaxConcurrency = maxConcurrency ?? 50
            };
        }

        private QueryRequestOptions GetDefaultQueryRequestOptions(string partitionKey)
        {
            return !string.IsNullOrWhiteSpace(partitionKey) ? 
            new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(partitionKey)
            }:
            null;
        }

        private async Task<IEnumerable<T>> ResultsFromQueryAndOptions<T>(CosmosDbQuery cosmosDbQuery, QueryRequestOptions queryOptions, int? maxItemCount = null)
        {
            return await Results<T>(cosmosDbQuery.CosmosQueryDefinition, queryOptions , maxItemCount);
        }

        private async Task<IEnumerable<T>> ResultsFromQueryAndOptions<T>(CosmosDbQuery cosmosDbQuery, Func<List<T>, Task> batchAction, QueryRequestOptions queryOptions)
        {
            return await Results(cosmosDbQuery.CosmosQueryDefinition, queryOptions, batchAction, queryOptions.MaxItemCount ?? 0);
        }

        private async Task<IEnumerable<T>> Results<T>(QueryDefinition queryDefinition, QueryRequestOptions queryRequestOptions = null, int? maxItemCount = null)
        {
            List<T> results = new List<T>();
            using FeedIterator feedIterator = RepositoryContainer.GetItemQueryStreamIterator(queryDefinition, requestOptions: queryRequestOptions);
            while (feedIterator.HasMoreResults)
            {
                foreach(T document in await CosmosDbFeedIterator.GetDocuments<T>(feedIterator))
                {
                    results.Add(document);

                    if (results.Count == maxItemCount)
                    {
                        return results;
                    }
                }
            }

            return results;
        }

        private int GetEffectivePageSize(int itemsPerPage, int? maxItemCount)
        {
            return itemsPerPage == -1 ? maxItemCount ?? itemsPerPage : Math.Min(maxItemCount ?? itemsPerPage, itemsPerPage);
        }

        private async Task<IEnumerable<T>> Results<T>(QueryDefinition queryDefinition, QueryRequestOptions queryRequestOptions, Func<List<T>, Task> batchAction, int itemsPerPage)
        {
            List<T> results = new List<T>();
            using FeedIterator feedIterator = RepositoryContainer.GetItemQueryStreamIterator(queryDefinition, requestOptions: queryRequestOptions);
            
            while (feedIterator.HasMoreResults)
            {
                results.AddRange(await CosmosDbFeedIterator.GetDocuments<T>(feedIterator));

                if (results.Count() >= itemsPerPage)
                {
                    await batchAction(results);
                    results.Clear();
                }
            }

            if (results.Any())
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
                yield return JsonConvert.SerializeObject(document); // haven't tried this yet!
            }
        }

        private async Task<ItemResponse<DocumentEntity<T>>> HardDeleteAsync<T>(T entity,
            string partitionKey,
            string etag = null) where T : IIdentifiable
        {
            PartitionKey partitionKeyForCosmos = string.IsNullOrWhiteSpace(partitionKey) ? PartitionKey.None : new PartitionKey(partitionKey);

            if (string.IsNullOrWhiteSpace(etag))
            {
                return await RepositoryContainer.DeleteItemAsync<DocumentEntity<T>>(id: entity.Id, partitionKey: partitionKeyForCosmos);
            }
            else
            {
                return await RepositoryContainer.DeleteItemAsync<DocumentEntity<T>>(id: entity.Id, partitionKey: partitionKeyForCosmos, new ItemRequestOptions
                {
                    IfMatchEtag = etag
                });
            }
        }

        private async Task<ItemResponse<DocumentEntity<T>>> SoftDeleteAsync<T>(T entity,
            PartitionKey partitionKey,
            string etag = null) where T : IIdentifiable
        {
            DocumentEntity<T> item = new DocumentEntity<T>(entity)
            {
                Deleted = true,
                UpdatedAt = DateTime.UtcNow,
                DocumentType = GetDocumentType<T>()
            };

            if (string.IsNullOrEmpty(etag))
            {
                return await RepositoryContainer.ReplaceItemAsync(item: item, id: entity.Id, partitionKey);
            }
            else
            {
                return await RepositoryContainer.ReplaceItemAsync(item: item, id: entity.Id, partitionKey, new ItemRequestOptions
                {
                    IfMatchEtag = etag
                });
            }
        }

        private async Task<HttpStatusCode> DeleteAsync<T>(T entity,
            string partitionKey,
            bool hardDelete = false,
            string etag = null) where T : IIdentifiable
        {
            ItemResponse<DocumentEntity<T>> response;

            if (hardDelete)
            {
                response = await HardDeleteAsync(entity, partitionKey, etag);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(partitionKey))
                {
                    response = await SoftDeleteAsync(entity, PartitionKey.None, etag);
                }
                else
                {
                    response = await SoftDeleteAsync(entity, new PartitionKey(partitionKey), etag);
                }
            }

            return response.StatusCode;
        }

        private static string GetDocumentType<T>()
        {
            return typeof(T).Name;
        }

        protected virtual CosmosClient GetClient(string connectionString, CosmosClientOptions cosmosClientOptions)
        {
            return CosmosDbConnectionString.Parse(connectionString, cosmosClientOptions);
        }

        public (bool Ok, string Message) IsHealthOk()
        {
            try
            {
                RepositoryClient.GetDatabase(_databaseName);
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private async Task CreateDatabaseIfNotExists()
        {
            if (RepositoryDatabase != null) return;

            DatabaseResponse databaseResponse = await RepositoryClient.CreateDatabaseIfNotExistsAsync(_databaseName);

            if (databaseResponse.StatusCode == HttpStatusCode.OK)
            {
                _database = databaseResponse.Database;
            }
            else
            {
                throw new Exception($"Database not created: {databaseResponse.StatusCode}");
            }
        }

        private async Task CreateContainerIfNotExists()
        {
            ContainerResponse containerResponse = await RepositoryDatabase.CreateContainerIfNotExistsAsync(
                id: _containerName,
                partitionKeyPath: _partitionKey,
                throughput: _defaultThroughput);

            if (containerResponse.StatusCode == HttpStatusCode.OK)
            {
                _container = containerResponse.Container;
            }
            else
            {
                throw new Exception($"Container not created: {containerResponse.StatusCode}");
            }
        }

        public async Task EnsureContainerExists()
        {
            if (RepositoryContainer == null)
            {
                await CreateDatabaseIfNotExists();

                await CreateContainerIfNotExists();
            }
        }

        public async Task<ThroughputResponse> SetThroughput(int requestUnits)
        {
            return await RepositoryContainer.ReplaceThroughputAsync(requestUnits);
        }

        public async Task<int?> GetThroughput()
        {
            return await RepositoryContainer.ReadThroughputAsync();
        }

        public async Task<int?> GetMinimumThroughput()
        {
            ThroughputResponse response = await RepositoryContainer.ReadThroughputAsync(new RequestOptions());

            return response?.MinThroughput;
        }

        public async Task<IEnumerable<DocumentEntity<T>>> Read<T>(int itemsPerPage = 1000) where T : IIdentifiable
        {
            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(itemsPerPage);

            QueryDefinition queryDefinition = RepositoryContainer
                .GetItemLinqQueryable<DocumentEntity<T>>()
                .Where(x => x.DocumentType == GetDocumentType<T>() && !x.Deleted)
                .ToQueryDefinition();

            return await Results<DocumentEntity<T>>(queryDefinition, queryRequestOptions);
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

            return (await Results<DocumentEntity<T>>(cosmosDbQuery.CosmosQueryDefinition)).SingleOrDefault();
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
                return default;
            }
        }

        public async Task<T> ReadByIdPartitionedAsync<T>(string id, string partitionKey) where T : IIdentifiable
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));
            Guard.IsNullOrWhiteSpace(partitionKey, nameof(partitionKey));

            ItemResponse<T> response = await RepositoryContainer.ReadItemAsync<T>(id: id, partitionKey: new PartitionKey(partitionKey));

            return response.Resource;
        }

        /// <summary>
        /// Read item by ID for partition and don't throw if not found
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="id">Cosmos item ID</param>
        /// <param name="partitionKey">Partition key</param>
        /// <returns>Document or default(T) if not found</returns>
        // As per https://github.com/Azure/azure-cosmos-dotnet-v3/issues/692
        public async Task<T> TryReadByIdPartitionedAsync<T>(string id, string partitionKey) where T : IIdentifiable
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));
            Guard.IsNullOrWhiteSpace(partitionKey, nameof(partitionKey));

            try
            {
                return await ReadByIdPartitionedAsync<T>(id, partitionKey);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
            {
                return default;
            }
        }

        public async Task<DocumentEntity<T>> ReadDocumentByIdPartitionedAsync<T>(string id, string partitionKey) where T : IIdentifiable
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));
            Guard.IsNullOrWhiteSpace(partitionKey, nameof(partitionKey));

            ItemResponse<DocumentEntity<T>> response = await RepositoryContainer.ReadItemAsync<DocumentEntity<T>>(id: id, partitionKey: new PartitionKey(partitionKey));

            return response.Resource;
        }

        /// <summary>
        /// Read item by ID for partition and don't throw if not found
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="id">Cosmos item ID</param>
        /// <param name="partitionKey">Partition key</param>
        /// <returns>Document or default(DocumentEntity<T>) if not found</returns>
        // As per https://github.com/Azure/azure-cosmos-dotnet-v3/issues/692
        public async Task<DocumentEntity<T>> TryReadDocumentByIdPartitionedAsync<T>(string id, string partitionKey) where T : IIdentifiable
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));
            Guard.IsNullOrWhiteSpace(partitionKey, nameof(partitionKey));

            try
            {
                return await ReadDocumentByIdPartitionedAsync<T>(id, partitionKey);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { }
            {
                return default;
            }
        }

        /// <summary>
        /// Query cosmos using IQueryable on a given entity.
        /// </summary>
        /// <typeparam name="T">Type of document stored in cosmos</typeparam>
        /// <returns></returns>
        public async Task<IEnumerable<T>> Query<T>(Expression<Func<DocumentEntity<T>, bool>> query = null, int itemsPerPage = -1, int? maxItemCount = null) where T : IIdentifiable
        {
            QueryRequestOptions queryRequestOptions = GetDefaultQueryRequestOptions(itemsPerPage: GetEffectivePageSize(itemsPerPage, maxItemCount));

            Expression<Func<DocumentEntity<T>, bool>> expression = x => x.DocumentType == GetDocumentType<T>() && !x.Deleted;

            QueryDefinition queryDefinition;

            if (query != null)
            {
                queryDefinition = RepositoryContainer.GetItemLinqQueryable<DocumentEntity<T>>()
                    .Where(expression)
                    .Where(query)
                    .ToQueryDefinition();
            }
            else
            {
                queryDefinition = RepositoryContainer.GetItemLinqQueryable<DocumentEntity<T>>()
                    .Where(expression)
                    .ToQueryDefinition();
            }

            return (await Results<DocumentEntity<T>>(queryDefinition, queryRequestOptions, maxItemCount: maxItemCount)).Select(x => x.Content);
        }

        public async Task<IEnumerable<T>> QueryPartitionedEntity<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, int? maxItemCount = null, string partitionKey = null) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryRequestOptions = GetDefaultQueryRequestOptions(itemsPerPage: GetEffectivePageSize(itemsPerPage, maxItemCount));
            queryRequestOptions.PartitionKey = new PartitionKey(partitionKey);

            IEnumerable<DocumentEntity<T>> documentResults = await ResultsFromQueryAndOptions<DocumentEntity<T>>(cosmosDbQuery, queryRequestOptions, maxItemCount);

            return documentResults.Select(x => x.Content);
        }

        public async Task<IEnumerable<T>> QuerySql<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, int? maxItemCount = null) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryOptions = GetQueryRequestOptions(GetEffectivePageSize(itemsPerPage, maxItemCount));

            IEnumerable<DocumentEntity<T>> results = await ResultsFromQueryAndOptions<DocumentEntity<T>>(cosmosDbQuery, queryOptions, maxItemCount);

            return results.Select(c => c.Content);
        }

        public ICosmosDbFeedIterator GetFeedIterator(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, int? maxItemCount = null)
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryOptions = GetQueryRequestOptions(GetEffectivePageSize(itemsPerPage, maxItemCount));

            return new CosmosDbFeedIterator(RepositoryContainer.GetItemQueryStreamIterator(
                queryDefinition: cosmosDbQuery.CosmosQueryDefinition,
                requestOptions: queryOptions));
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

        public async Task<IEnumerable<T>> RawQuery<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, int? maxItemCount = null)
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryOptions = GetDefaultQueryRequestOptions(itemsPerPage: itemsPerPage,
                maxBufferedItemCount: 50,
                maxConcurrency: 100);

            return await ResultsFromQueryAndOptions<T>(cosmosDbQuery, queryOptions, maxItemCount);
        }

        public async Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(int itemsPerPage = 1000, Expression<Func<DocumentEntity<T>, bool>> query = null) where T : IIdentifiable
        {
            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(itemsPerPage);

            QueryDefinition queryDefinition;

            if (query == null)
            {
                queryDefinition = RepositoryContainer
                    .GetItemLinqQueryable<DocumentEntity<T>>()
                    .Where(d => d.DocumentType == GetDocumentType<T>())
                    .ToQueryDefinition();
            }
            else
            {
                queryDefinition = RepositoryContainer
                    .GetItemLinqQueryable<DocumentEntity<T>>()
                    .Where(query)
                    .ToQueryDefinition();
            }

            return await Results<DocumentEntity<T>>(queryDefinition, queryRequestOptions);
        }

        public async Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(CosmosDbQuery cosmosDbQuery, int itemsPerPage = 1000) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(itemsPerPage);

            return await Results<DocumentEntity<T>>(cosmosDbQuery.CosmosQueryDefinition, queryRequestOptions);
        }

        public async Task DocumentsBatchProcessingAsync<T>(Func<List<DocumentEntity<T>>, Task> persistBatchToIndex, int itemsPerPage = 1000, Expression<Func<DocumentEntity<T>, bool>> query = null) where T : IIdentifiable
        {
            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(itemsPerPage);

            IQueryable<DocumentEntity<T>> allResults;

            IOrderedQueryable<DocumentEntity<T>> queryable = RepositoryContainer.GetItemLinqQueryable<DocumentEntity<T>>(allowSynchronousQueryExecution: true, requestOptions: queryRequestOptions);

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
        
        public IQueryable<DocumentEntity<T>> QueryableDocuments<T>(int itemsPerPage = -1, int? maxItemCount = null) where T : IIdentifiable
        {
            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(GetEffectivePageSize(itemsPerPage, maxItemCount));

            return RepositoryContainer
                .GetItemLinqQueryable<DocumentEntity<T>>(allowSynchronousQueryExecution: true, requestOptions: queryRequestOptions)
                .Where(x => x.DocumentType == GetDocumentType<T>() && !x.Deleted);
        }

        public async Task<IEnumerable<DocumentEntity<T>>> QueryDocuments<T>(int itemsPerPage = -1, int? maxItemCount = null) where T : IIdentifiable
        {
            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(GetEffectivePageSize(itemsPerPage, maxItemCount));

            QueryDefinition queryDefinition = RepositoryContainer
                .GetItemLinqQueryable<DocumentEntity<T>>()
                .Where(x => x.DocumentType == GetDocumentType<T>() && !x.Deleted)
                .ToQueryDefinition();

            return await Results<DocumentEntity<T>>(queryDefinition, queryRequestOptions, maxItemCount: maxItemCount);
        }

        public async Task<IEnumerable<string>> QueryAsJson(int itemsPerPage = -1, int? maxItemCount = null)
        {
            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(GetEffectivePageSize(itemsPerPage, maxItemCount));

            QueryDefinition queryDefinition = RepositoryContainer
                .GetItemLinqQueryable<Document>()
                .ToQueryDefinition();

            IEnumerable<Document> documents = await Results<Document>(queryDefinition, queryRequestOptions, maxItemCount: maxItemCount);

            return JsonFromDocuments(documents);
        }

        public async Task<IEnumerable<string>> QueryAsJsonAsync(CosmosDbQuery cosmosDbQuery, int itemsPerPage = -1, int? maxItemCount = null)
        {
            Guard.ArgumentNotNull(cosmosDbQuery, nameof(cosmosDbQuery));

            QueryRequestOptions queryRequestOptions = GetQueryRequestOptions(GetEffectivePageSize(itemsPerPage, maxItemCount));

            IEnumerable<Document> documents = await ResultsFromQueryAndOptions<Document>(cosmosDbQuery, queryRequestOptions, maxItemCount);

            return JsonFromDocuments(documents);
        }

        public async Task<HttpStatusCode> DeleteAsync<T>(string id,
            string partitionKey,
            bool hardDelete = false,
            string etag = null) where T : IIdentifiable
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));

            DocumentEntity<T> doc = await ReadDocumentByIdAsync<T>(id);

            return await DeleteAsync(doc, partitionKey, hardDelete, etag);
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

        private async Task<ItemResponse<DocumentEntity<T>>> CreateDocumentInternalAsync<T>(T entity, ItemRequestOptions requestOptions = null) where T : IIdentifiable
        {
            DocumentEntity<T> doc = CreateDocumentEntity(entity);

            return await RepositoryContainer.CreateItemAsync(doc, requestOptions: requestOptions);
        }

        public async Task<HttpStatusCode> CreateAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            ItemResponse<DocumentEntity<T>> response = await CreateDocumentInternalAsync(entity, _disableContentResponseOnWriteRequestOptions);

            return response.StatusCode;
        }

        public async Task<DocumentEntity<T>> CreateDocumentAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            ItemResponse<DocumentEntity<T>> response = await CreateDocumentInternalAsync(entity);

            return response.Resource;
        }

        public async Task<HttpStatusCode> UpsertAsync<T>(T entity,
            string partitionKey = null,
            bool undelete = false,
            bool maintainCreatedDate = true,
            string etag = null) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            QueryRequestOptions queryRequestOptions = GetDefaultQueryRequestOptions(partitionKey: partitionKey);

            DocumentEntity<T> doc;

            if (maintainCreatedDate)
            {
                //SingleOrDefault not supported on the current Cosmos driver
                List<DocumentEntity<T>> documents = RepositoryContainer
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

            if (string.IsNullOrWhiteSpace(etag))
            {
                ItemResponse<DocumentEntity<T>> response = await RepositoryContainer.UpsertItemAsync(doc, requestOptions: _disableContentResponseOnWriteRequestOptions);
                return response.StatusCode;
            }
            else
            {
                ItemResponse<DocumentEntity<T>> response = await RepositoryContainer.UpsertItemAsync(doc, requestOptions: new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false,
                    IfMatchEtag = etag
                });
                return response.StatusCode;   
            }
        }

        public async Task<HttpStatusCode> CreateAsync<T>(KeyValuePair<string, T> entity) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entity, nameof(entity));
            Guard.ArgumentNotNull(entity.Value, nameof(entity.Value));

            DocumentEntity<T> doc = CreateDocumentEntity(entity.Value);

            ItemResponse<DocumentEntity<T>> response = await RepositoryContainer.CreateItemAsync(item: doc, partitionKey: new PartitionKey(entity.Key), _disableContentResponseOnWriteRequestOptions);
            return response.StatusCode;
        }

        public async Task<ItemResponse<DocumentEntity<T>>> CreateWithResponseAsync<T>(T entity) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entity, nameof(entity));

            DocumentEntity<T> doc = CreateDocumentEntity(entity);

            return await RepositoryContainer.CreateItemAsync(doc);
        }

        public async Task BulkCreateAsync<T>(IList<T> entities, int degreeOfParallelism = 5) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entities, nameof(entities));

            List<Task> allTasks = new List<Task>(entities.Count);
            SemaphoreSlim throttler = new SemaphoreSlim(degreeOfParallelism);

            foreach (T entity in entities)
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

            await TaskHelper.WhenAllAndThrow(allTasks.ToArray());
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

            await TaskHelper.WhenAllAndThrow(allTasks.ToArray());
        }

        public async Task BulkDeleteAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5, bool hardDelete = false) where T : IIdentifiable
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
                            await DeleteAsync(entity: entity.Value, partitionKey: entity.Key, hardDelete: hardDelete);
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
            }

            await TaskHelper.WhenAllAndThrow(allTasks.ToArray());
        }

        public async Task BulkUpsertAsync<T>(IList<T> entities, int degreeOfParallelism = 5, bool maintainCreatedDate = true, bool undelete = false) where T : IIdentifiable
        {
            Guard.ArgumentNotNull(entities, nameof(entities));

            List<Task> allTasks = new List<Task>(entities.Count());
            SemaphoreSlim throttler = new SemaphoreSlim(initialCount: degreeOfParallelism);

            foreach (T entity in entities)
            {
                await throttler.WaitAsync();
                allTasks.Add(
                    Task.Run(async () =>
                    {
                        try
                        {
                            await UpsertAsync(entity, entity.Id, undelete: undelete, maintainCreatedDate: maintainCreatedDate);
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
            }

            await TaskHelper.WhenAllAndThrow(allTasks.ToArray());
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
                            await UpsertAsync(entity: entity.Value, partitionKey: entity.Key, undelete: undelete, maintainCreatedDate: maintainCreatedDate);
                        }
                        finally
                        {
                            throttler.Release();
                        }
                    }));
            }
            await TaskHelper.WhenAllAndThrow(allTasks.ToArray());
        }

        public async Task<HttpStatusCode> UpdateAsync<T>(T entity,
            bool undelete = false,
            string etag = null) where T : Reference
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

            if (string.IsNullOrWhiteSpace(etag))
            {
                ItemResponse<DocumentEntity<T>> response = await RepositoryContainer.ReplaceItemAsync(item: doc, id: entity.Id, requestOptions: _disableContentResponseOnWriteRequestOptions);
                return response.StatusCode;
            }
            else
            {
                ItemResponse<DocumentEntity<T>> response = await RepositoryContainer.ReplaceItemAsync(item: doc, id: entity.Id, requestOptions: new ItemRequestOptions
                {
                    EnableContentResponseOnWrite = false,
                    IfMatchEtag = etag
                });
                return response.StatusCode;   
            }
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

            Scripts cosmosScripts = RepositoryContainer.Scripts;

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
