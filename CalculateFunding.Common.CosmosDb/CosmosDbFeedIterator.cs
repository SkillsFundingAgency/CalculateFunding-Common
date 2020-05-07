using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.Models;
using Microsoft.Azure.Cosmos;

namespace CalculateFunding.Common.CosmosDb
{
    public class CosmosDbFeedIterator<TDocument> : ICosmosDbFeedIterator<TDocument>
        where TDocument : IIdentifiable
    {
        private readonly FeedIterator<DocumentEntity<TDocument>> _feedIterator;

        protected internal CosmosDbFeedIterator(FeedIterator<DocumentEntity<TDocument>> feedIterator)
        {
            _feedIterator = feedIterator;
        }

        public bool HasMoreResults => _feedIterator.HasMoreResults;

        public async Task<IEnumerable<TDocument>> ReadNext(CancellationToken cancellationToken = default)
        {
            return (await _feedIterator.ReadNextAsync(cancellationToken)).Select(_ => _.Content);
        }
    }
}