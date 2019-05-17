using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.Models;
using CalculateFunding.Common.Utility;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace CalculateFunding.Common.CosmosDb
{
    public class CosmosRepository : ICosmosRepository, IDisposable
    {
        private readonly string _collectionName;
        private readonly string _partitionKey;
        private readonly string _databaseName;
        private readonly DocumentClient _documentClient;
        private readonly Uri _collectionUri;
        private ResourceResponse<DocumentCollection> _collection;

        public CosmosRepository(CosmosDbSettings settings)
        {
            Guard.ArgumentNotNull(settings, nameof(settings));
            Guard.IsNullOrWhiteSpace(settings.CollectionName, nameof(settings.CollectionName));
            Guard.IsNullOrWhiteSpace(settings.ConnectionString, nameof(settings.ConnectionString));
            Guard.IsNullOrWhiteSpace(settings.DatabaseName, nameof(settings.DatabaseName));

            _collectionName = settings.CollectionName;
            _partitionKey = settings.PartitionKey;
            _databaseName = settings.DatabaseName;
            _documentClient = CosmosDbConnectionString.Parse(settings.ConnectionString);
            _collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName);
        }

        public async Task<(bool Ok, string Message)> IsHealthOk()
        {
            try
            {
                await _documentClient.OpenAsync();
                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task EnsureCollectionExists()
        {
            if (_collection == null)
            {
                DocumentCollection collection = new DocumentCollection { Id = _collectionName };
                if (_partitionKey != null)
                {
                    collection.PartitionKey.Paths.Add(_partitionKey);
                }

                try
                {
                    await _documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseName));
                }
                catch (DocumentClientException)
                {
                    await _documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseName });
                }

                _collection = await _documentClient.CreateDocumentCollectionIfNotExistsAsync(
                    UriFactory.CreateDatabaseUri(_databaseName), collection);
            }
        }

        public async Task SetThroughput(int requestUnits)
        {
            //Fetch the resource to be updated
            OfferV2 offer = (OfferV2)_documentClient.CreateOfferQuery()
                .Where(r => r.ResourceLink == _collection.Resource.SelfLink)
                .SingleOrDefault();

            if (offer.Content.OfferThroughput != requestUnits)
            {
                // Set the throughput to the new value, for example 12,000 request units per second
                offer = new OfferV2(offer, requestUnits);

                //Now persist these changes to the database by replacing the original resource
                await _documentClient.ReplaceOfferAsync(offer);
            }
        }

        private static string GetDocumentType<T>()
        {
            return typeof(T).Name;
        }

        public IQueryable<DocumentEntity<T>> Read<T>(int itemsPerPage = 1000) where T : IIdentifiable
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = itemsPerPage };

            return _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, queryOptions).Where(x => x.DocumentType == GetDocumentType<T>() && !x.Deleted);
        }

        public async Task<DocumentEntity<T>> ReadAsync<T>(string id) where T : IIdentifiable
        {
            FeedResponse<DocumentEntity<T>> response = await Read<T>(itemsPerPage: 1).Where(x => x.Id == id).AsDocumentQuery().ExecuteNextAsync<DocumentEntity<T>>();
            return response.FirstOrDefault();
        }

        /// <summary>
        /// Query cosmos using IQueryable on a given entity.
        /// NOTE: The directSql may not work, only linq queries
        /// </summary>
        /// <typeparam name="T">Type of document stored in cosmos</typeparam>
        /// <param name="directSql">Direct SQL Query - may not work</param>
        /// <param name="enableCrossPartitionQuery">Enable cross partitioned query</param>
        /// <returns></returns>
        [Obsolete]
        public IQueryable<T> Query<T>(string directSql = null, bool enableCrossPartitionQuery = false) where T : IIdentifiable
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = enableCrossPartitionQuery,
                MaxBufferedItemCount = 100,
                MaxDegreeOfParallelism = 50,
            };

            if (!string.IsNullOrEmpty(directSql))
            {
                // This probably doesn't work - it may need an .AsDocumentQuery() before the .Select
                return _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri,
                    directSql,
                    queryOptions).Select(x => x.Content);
            }

            return _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, queryOptions).Where(x => x.DocumentType == GetDocumentType<T>() && !x.Deleted).Select(x => x.Content);
        }

        /// <summary>
        /// Query cosmos using IQueryable on a given entity.
        /// NOTE: The directSql may not work, only linq queries
        /// </summary>
        /// <typeparam name="T">Type of document stored in cosmos</typeparam>
        /// <param name="directSql">Direct SQL Query - may not work</param>
        /// <param name="enableCrossPartitionQuery">Enable cross partitioned query</param>
        /// <returns></returns>
        public IQueryable<T> Query<T>(SqlQuerySpec sqlQuerySpec = null, bool enableCrossPartitionQuery = false) where T : IIdentifiable
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = enableCrossPartitionQuery,
                MaxBufferedItemCount = 100,
                MaxDegreeOfParallelism = 50,
            };

            if (sqlQuerySpec != null)
            {
                // This probably doesn't work - it may need an .AsDocumentQuery() before the .Select
                return _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri,
                    sqlQuerySpec,
                    queryOptions).Select(x => x.Content);
            }

            return _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, queryOptions).Where(x => x.DocumentType == GetDocumentType<T>() && !x.Deleted).Select(x => x.Content);
        }

        [Obsolete]
        public async Task<IEnumerable<T>> QueryPartitionedEntity<T>(string directSql, int itemsPerPage = -1, string partitionEntityId = null) where T : IIdentifiable
        {
            if (string.IsNullOrEmpty(directSql))
            {
                throw new ArgumentNullException(nameof(directSql));
            }

            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                MaxItemCount = itemsPerPage,
                EnableCrossPartitionQuery = false,
                PartitionKey = new PartitionKey(partitionEntityId),
                MaxDegreeOfParallelism = 50,
                MaxBufferedItemCount = 100,
            };

            return (await _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri,
                directSql,
                queryOptions).AsDocumentQuery().ExecuteNextAsync<DocumentEntity<T>>()).Select(x => x.Content);
        }

        public async Task<IEnumerable<T>> QueryPartitionedEntity<T>(SqlQuerySpec sqlQuerySpec, int itemsPerPage = -1, string partitionEntityId = null) where T : IIdentifiable
        {
            if (sqlQuerySpec == null) throw new ArgumentNullException(nameof(sqlQuerySpec));

            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                MaxItemCount = itemsPerPage,
                EnableCrossPartitionQuery = false,
                PartitionKey = new PartitionKey(partitionEntityId),
                MaxDegreeOfParallelism = 50,
                MaxBufferedItemCount = 100,
            };

            return (await _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri,
                sqlQuerySpec,
                queryOptions).AsDocumentQuery().ExecuteNextAsync<DocumentEntity<T>>()).Select(x => x.Content);
        }

        [Obsolete]
        public IQueryable<dynamic> DynamicQuery<dynamic>(string sql, bool enableCrossPartitionQuery = false)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = enableCrossPartitionQuery,
            };

            IQueryable<dynamic> query = _documentClient.CreateDocumentQuery<dynamic>(_collectionUri, sql, queryOptions);

            return query;
        }

        public IQueryable<dynamic> DynamicQuery<dynamic>(SqlQuerySpec sqlQuerySpec, bool enableCrossPartitionQuery = false)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = enableCrossPartitionQuery,
            };

            IQueryable<dynamic> query = _documentClient.CreateDocumentQuery<dynamic>(_collectionUri, sqlQuerySpec, queryOptions);

            return query;
        }

        [Obsolete]
        public IQueryable<dynamic> DynamicQueryPartionedEntity<dynamic>(string sql, string partitionEntityId = null)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = false,
                PartitionKey = new PartitionKey(partitionEntityId),
                MaxDegreeOfParallelism = 50,
                MaxBufferedItemCount = 100,
            };

            IQueryable<dynamic> query = _documentClient.CreateDocumentQuery<dynamic>(_collectionUri, sql, queryOptions);

            return query;
        }

        public IQueryable<dynamic> DynamicQueryPartionedEntity<dynamic>(SqlQuerySpec sqlQuerySpec, string partitionEntityId = null)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = false,
                PartitionKey = new PartitionKey(partitionEntityId),
                MaxDegreeOfParallelism = 50,
                MaxBufferedItemCount = 100,
            };

            IQueryable<dynamic> query = _documentClient.CreateDocumentQuery<dynamic>(_collectionUri, sqlQuerySpec, queryOptions);

            return query;
        }

        [Obsolete]
        public async Task<IEnumerable<dynamic>> QueryDynamic(string sql, bool enableCrossPartitionQuery = false, int itemsPerPage = 1000)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = enableCrossPartitionQuery,
                MaxItemCount = itemsPerPage,
                MaxDegreeOfParallelism = 50,
                MaxBufferedItemCount = 100,
            };

            IEnumerable<dynamic> results = new List<dynamic>();

            IDocumentQuery<dynamic> queryable = _documentClient.CreateDocumentQuery<dynamic>(_collectionUri, sql, queryOptions).AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                results = results.Concat(await queryable.ExecuteNextAsync<dynamic>());
            }

            return results;
        }

        public async Task<IEnumerable<dynamic>> QueryDynamic(SqlQuerySpec sqlQuerySpec, bool enableCrossPartitionQuery = false, int itemsPerPage = 1000)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = enableCrossPartitionQuery,
                MaxItemCount = itemsPerPage,
                MaxDegreeOfParallelism = 50,
                MaxBufferedItemCount = 100,
            };

            IEnumerable<dynamic> results = new List<dynamic>();

            IDocumentQuery<dynamic> queryable = _documentClient.CreateDocumentQuery<dynamic>(_collectionUri, sqlQuerySpec, queryOptions).AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                results = results.Concat(await queryable.ExecuteNextAsync<dynamic>());
            }

            return results;
        }

        [Obsolete]
        public IQueryable<T> RawQuery<T>(string directSql, int itemsPerPage = -1, bool enableCrossPartitionQuery = false)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                MaxItemCount = itemsPerPage,
                EnableCrossPartitionQuery = enableCrossPartitionQuery,
                MaxDegreeOfParallelism = 50,
                MaxBufferedItemCount = 100,
            };

            return _documentClient.CreateDocumentQuery<T>(_collectionUri,
                directSql,
                queryOptions).AsQueryable();
        }

        public IQueryable<T> RawQuery<T>(SqlQuerySpec sqlQuerySpec, int itemsPerPage = -1, bool enableCrossPartitionQuery = false)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                MaxItemCount = itemsPerPage,
                EnableCrossPartitionQuery = enableCrossPartitionQuery,
                MaxDegreeOfParallelism = 50,
                MaxBufferedItemCount = 100,
            };

            return _documentClient.CreateDocumentQuery<T>(_collectionUri,
                sqlQuerySpec,
                queryOptions).AsQueryable();
        }

        [Obsolete]
        public async Task<IEnumerable<T>> QuerySql<T>(string directSql, int itemsPerPage = -1, bool enableCrossPartitionQuery = false) where T : IIdentifiable
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = enableCrossPartitionQuery,
                MaxItemCount = itemsPerPage
            };

            List<T> results = new List<T>();

            IDocumentQuery<DocumentEntity<T>> queryable = _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, directSql, queryOptions).AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                FeedResponse<DocumentEntity<T>> queryResponse = await queryable.ExecuteNextAsync<DocumentEntity<T>>();

                results.AddRange(queryResponse.Select(s => s.Content));
            }

            return results;
        }

        public async Task<IEnumerable<T>> QuerySql<T>(SqlQuerySpec sqlQuerySpec, int itemsPerPage = -1, bool enableCrossPartitionQuery = false) where T : IIdentifiable
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions
            {
                EnableCrossPartitionQuery = enableCrossPartitionQuery,
                MaxItemCount = itemsPerPage
            };

            List<T> results = new List<T>();

            IDocumentQuery<DocumentEntity<T>> queryable = _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, sqlQuerySpec, queryOptions).AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                FeedResponse<DocumentEntity<T>> queryResponse = await queryable.ExecuteNextAsync<DocumentEntity<T>>();

                results.AddRange(queryResponse.Select(s => s.Content));
            }

            return results;
        }

        public async Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(int itemsPerPage = 1000, Expression<Func<DocumentEntity<T>, bool>> query = null, bool enableCrossPartitionQuery = true) where T : IIdentifiable
        {
            FeedOptions options = new FeedOptions() { MaxItemCount = itemsPerPage, EnableCrossPartitionQuery = enableCrossPartitionQuery };

            List<DocumentEntity<T>> allResults = new List<DocumentEntity<T>>();

            IDocumentQuery<DocumentEntity<T>> queryable = null;

            if (query == null)
            {
                queryable = _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, options)
                    .Where(d => d.DocumentType == GetDocumentType<T>())
                    .AsDocumentQuery();
            }
            else
            {
                queryable = _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, options)
                    .Where(query)
                    .AsDocumentQuery();
            }

            while (queryable.HasMoreResults)
            {
                allResults.AddRange(await queryable.ExecuteNextAsync<DocumentEntity<T>>());
            }

            return allResults;
        }

        [Obsolete]
        public async Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(string sql, int itemsPerPage = 1000, bool enableCrossPartitionQuery = true) where T : IIdentifiable
        {
            FeedOptions options = new FeedOptions() { MaxItemCount = itemsPerPage, EnableCrossPartitionQuery = enableCrossPartitionQuery };

            List<DocumentEntity<T>> allResults = new List<DocumentEntity<T>>();

            IDocumentQuery<DocumentEntity<T>> queryable = null;

            queryable = _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, sql, options).AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                allResults.AddRange(await queryable.ExecuteNextAsync<DocumentEntity<T>>());
            }

            return allResults;
        }

        public async Task<IEnumerable<DocumentEntity<T>>> GetAllDocumentsAsync<T>(SqlQuerySpec sqlQuerySpec, int itemsPerPage = 1000, bool enableCrossPartitionQuery = true) where T : IIdentifiable
        {
            FeedOptions options = new FeedOptions { MaxItemCount = itemsPerPage, EnableCrossPartitionQuery = enableCrossPartitionQuery };

            List<DocumentEntity<T>> allResults = new List<DocumentEntity<T>>();

            IDocumentQuery<DocumentEntity<T>> queryable = null;

            queryable = _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, sqlQuerySpec, options).AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                allResults.AddRange(await queryable.ExecuteNextAsync<DocumentEntity<T>>());
            }

            return allResults;
        }

        public async Task DocumentsBatchProcessingAsync<T>(Func<List<DocumentEntity<T>>, Task> persistBatchToIndex, int itemsPerPage = 1000, Expression<Func<DocumentEntity<T>, bool>> query = null) where T : IIdentifiable
        {
            FeedOptions options = new FeedOptions() { MaxItemCount = itemsPerPage, EnableCrossPartitionQuery = true };

            List<DocumentEntity<T>> allResults = new List<DocumentEntity<T>>();

            IDocumentQuery<DocumentEntity<T>> queryable = null;

            if (query == null)
            {
                queryable = _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, options)
                    .Where(d => d.DocumentType == GetDocumentType<T>())
                    .AsDocumentQuery();
            }
            else
            {
                queryable = _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, options)
                    .Where(query)
                    .AsDocumentQuery();
            }

            while (queryable.HasMoreResults)
            {
                if (allResults.Count() >= itemsPerPage)
                {
                    await persistBatchToIndex(allResults);
                    allResults.Clear();
                }

                allResults.AddRange(await queryable.ExecuteNextAsync<DocumentEntity<T>>());
            }

            if (allResults.Count() > 0)
            {
                await persistBatchToIndex(allResults);
                allResults.Clear();
            }
        }

        [Obsolete]
        public async Task DocumentsBatchProcessingAsync<T>(Func<List<T>, Task> persistBatchToIndex, string sql, int itemsPerPage = 1000) where T : IIdentifiable
        {
            FeedOptions options = new FeedOptions() { MaxItemCount = itemsPerPage, EnableCrossPartitionQuery = true };

            List<T> results = new List<T>();

            IDocumentQuery<T> queryable = _documentClient.CreateDocumentQuery<T>(_collectionUri, sql, options).AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                if (results.Count() >= itemsPerPage)
                {
                    await persistBatchToIndex(results);
                    results.Clear();
                }

                results.AddRange(await queryable.ExecuteNextAsync<T>());
            }

            if (results.Count() > 0)
            {
                await persistBatchToIndex(results);
                results.Clear();
            }
        }

        public async Task DocumentsBatchProcessingAsync<T>(Func<List<T>, Task> persistBatchToIndex, SqlQuerySpec sqlQuerySpec, int itemsPerPage = 1000) where T : IIdentifiable
        {
            FeedOptions options = new FeedOptions() { MaxItemCount = itemsPerPage, EnableCrossPartitionQuery = true };

            List<T> results = new List<T>();

            IDocumentQuery<T> queryable = _documentClient.CreateDocumentQuery<T>(_collectionUri, sqlQuerySpec, options).AsDocumentQuery();

            while (queryable.HasMoreResults)
            {
                if (results.Count >= itemsPerPage)
                {
                    await persistBatchToIndex(results);
                    results.Clear();
                }

                results.AddRange(await queryable.ExecuteNextAsync<T>());
            }

            if (results.Any())
            {
                await persistBatchToIndex(results);
                results.Clear();
            }
        }

        [Obsolete]
        public IQueryable<DocumentEntity<T>> QueryDocuments<T>(string directSql = null, int itemsPerPage = -1) where T : IIdentifiable
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = itemsPerPage };

            if (!string.IsNullOrEmpty(directSql))
            {
                return _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri,
                    directSql,
                    queryOptions).AsQueryable();
            }

            return _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, queryOptions).AsQueryable();
        }

        public IQueryable<DocumentEntity<T>> QueryDocuments<T>(SqlQuerySpec sqlQuerySpec, int itemsPerPage = -1) where T : IIdentifiable
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = itemsPerPage };

            if (sqlQuerySpec != null)
            {
                return _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri,
                    sqlQuerySpec,
                    queryOptions).AsQueryable();
            }

            return _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, queryOptions).AsQueryable();
        }

        [Obsolete]
        public IEnumerable<string> QueryAsJson(string directSql = null, int itemsPerPage = -1)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = itemsPerPage };

            IEnumerable<Document> documents = _documentClient.CreateDocumentQuery<Document>(_collectionUri, directSql, queryOptions).ToArray();
            foreach (Document document in documents)
            {
                dynamic json = document;
                yield return JsonConvert.SerializeObject(json.Content); // haven't tried this yet!
            }
        }

        public IEnumerable<string> QueryAsJson(SqlQuerySpec sqlQuerySpec = null, int itemsPerPage = -1)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = itemsPerPage };

            IEnumerable<Document> documents = _documentClient.CreateDocumentQuery<Document>(_collectionUri, sqlQuerySpec, queryOptions).ToArray();
            foreach (Document document in documents)
            {
                dynamic json = document;
                yield return JsonConvert.SerializeObject(json.Content); // haven't tried this yet!
            }
        }

        public async Task<HttpStatusCode> DeleteAsync<T>(string id) where T : IIdentifiable
        {
            DocumentEntity<T> doc = await ReadAsync<T>(id);
            doc.Deleted = true;
            ResourceResponse<Document> response = await _documentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, doc.Id), doc);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> CreateAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable
        {
            DocumentEntity<T> doc = new DocumentEntity<T>(entity)
            {
                DocumentType = GetDocumentType<T>(),
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            };

            ResourceResponse<Document> response = await _documentClient.CreateDocumentAsync(_collectionUri, doc);
            return response.StatusCode;
        }

        public async Task<DocumentEntity<T>> CreateDocumentAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable
        {
            DocumentEntity<T> doc = new DocumentEntity<T>(entity)
            {
                DocumentType = GetDocumentType<T>(),
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            };

            ResourceResponse<Document> response = await _documentClient.CreateDocumentAsync(_collectionUri, doc);

            return doc;
        }

        public async Task<HttpStatusCode> UpsertAsync<T>(T entity, string partitionKey = null) where T : IIdentifiable
        {
            FeedOptions feedOptions = new FeedOptions()
            {
                PartitionKey = string.IsNullOrWhiteSpace(partitionKey) ? null : new PartitionKey(partitionKey)
            };

            DocumentEntity<T> doc = _documentClient.CreateDocumentQuery<DocumentEntity<T>>(_collectionUri, feedOptions).Where(d => d.Id == entity.Id).SingleOrDefault();

            if (doc == null)
            {
                doc = new DocumentEntity<T>(entity)
                {
                    DocumentType = GetDocumentType<T>(),
                    CreatedAt = DateTimeOffset.Now,
                    UpdatedAt = DateTimeOffset.Now
                };
            }
            else
            {
                doc.Content = entity;
                doc.UpdatedAt = DateTimeOffset.Now;
            }

            ResourceResponse<Document> response = await _documentClient.UpsertDocumentAsync(_collectionUri, doc);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> CreateAsync<T>(KeyValuePair<string, T> entity) where T : IIdentifiable
        {
            DocumentEntity<T> doc = new DocumentEntity<T>(entity.Value)
            {
                DocumentType = GetDocumentType<T>(),
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            };

            RequestOptions options = new RequestOptions()
            {
                PartitionKey = new PartitionKey(entity.Key),
            };

            ResourceResponse<Document> response = await _documentClient.CreateDocumentAsync(_collectionUri, doc, options);
            return response.StatusCode;
        }

        public Task<ResourceResponse<Document>> CreateWithResponseAsync<T>(T entity) where T : IIdentifiable
        {
            DocumentEntity<T> doc = new DocumentEntity<T>(entity)
            {
                DocumentType = GetDocumentType<T>(),
                CreatedAt = DateTimeOffset.Now,
                UpdatedAt = DateTimeOffset.Now
            };
            return _documentClient.CreateDocumentAsync(_collectionUri, doc);
        }

        public async Task BulkCreateAsync<T>(IList<T> entities, int degreeOfParallelism = 5) where T : IIdentifiable
        {
            await Task.Run(() => Parallel.ForEach(entities, new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism }, (item) =>
            {
                Task.WaitAll(CreateAsync(item));
            }));
        }

        public async Task BulkCreateAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5) where T : IIdentifiable
        {
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

        public async Task BulkUpsertAsync<T>(IList<T> entities, int degreeOfParallelism = 5) where T : IIdentifiable
        {
            await Task.Run(() => Parallel.ForEach(entities, new ParallelOptions { MaxDegreeOfParallelism = degreeOfParallelism }, (item) =>
            {
                Task.WaitAll(UpsertAsync(item));
            }));
        }

        public async Task BulkUpsertAsync<T>(IEnumerable<KeyValuePair<string, T>> entities, int degreeOfParallelism = 5) where T : IIdentifiable
        {
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
                            await UpsertAsync(entity.Value, entity.Key);
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

        public async Task<HttpStatusCode> UpdateAsync<T>(T entity) where T : Reference
        {
            string documentType = GetDocumentType<T>();
            DocumentEntity<T> doc = new DocumentEntity<T>(entity);
            if (doc.DocumentType != null && doc.DocumentType != documentType)
            {
                throw new ArgumentException($"Cannot change {entity.Id} from {doc.DocumentType} to {typeof(T).Name}");
            }
            doc.DocumentType = documentType; // in case not specified
            doc.UpdatedAt = DateTimeOffset.Now;
            ResourceResponse<Document> response = await _documentClient.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, entity.Id), doc);
            return response.StatusCode;
        }

        public async Task<HttpStatusCode> BulkUpdateAsync<T>(IEnumerable<T> entities, string storedProcedureName) where T : IIdentifiable
        {
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

            try
            {
                string documentsAsJson = JsonConvert.SerializeObject(documents);

                dynamic[] args = new dynamic[] { JsonConvert.DeserializeObject<dynamic>(documentsAsJson) };

                Uri link = UriFactory.CreateStoredProcedureUri(_databaseName, _collectionName, storedProcedureName);

                StoredProcedureResponse<string> result = await _documentClient.ExecuteStoredProcedureAsync<string>
                     (link, args);

                return result.StatusCode;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _documentClient?.Dispose();
            }
        }
    }
}
