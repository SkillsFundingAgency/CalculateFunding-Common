using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CalculateFunding.Common.CosmosDb
{
    public interface ICosmosDbFeedIterator<TDocument>
    {
        bool HasMoreResults { get; }

        Task<IEnumerable<TDocument>> ReadNext(CancellationToken cancellationToken = default);
    }
}