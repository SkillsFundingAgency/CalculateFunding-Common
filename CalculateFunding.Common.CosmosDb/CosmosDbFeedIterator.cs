using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.Models;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

namespace CalculateFunding.Common.CosmosDb
{
    public class CosmosDbFeedIterator : ICosmosDbFeedIterator
    {
        private readonly FeedIterator _feedIterator;

        protected internal CosmosDbFeedIterator(FeedIterator feedIterator)
        {
            _feedIterator = feedIterator;
        }

        public bool HasMoreResults => _feedIterator.HasMoreResults;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _feedIterator?.Dispose();
            }
        }

        public async Task<IEnumerable<TDocument>> ReadNext<TDocument>(CancellationToken cancellationToken = default)
            where TDocument : IIdentifiable
        {
            return (await GetDocuments<DocumentEntity<TDocument>>(_feedIterator)).Select(_ => _.Content);
        }

        internal static async Task<IEnumerable<T>> GetDocuments<T>(FeedIterator feedIterator)
        {
            using ResponseMessage response = await feedIterator.ReadNextAsync();
            response.EnsureSuccessStatusCode();
            using StreamReader sr = new StreamReader(response.Content);
            using JsonTextReader jtr = new JsonTextReader(sr);
            JsonSerializer jsonSerializer = new JsonSerializer();
            QueryStream<T> array = jsonSerializer.Deserialize<QueryStream<T>>(jtr);
            return array.Documents;
        }
    }
}