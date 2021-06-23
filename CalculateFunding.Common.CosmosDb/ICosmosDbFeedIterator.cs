using CalculateFunding.Common.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CalculateFunding.Common.CosmosDb
{
    public interface ICosmosDbFeedIterator : IDisposable
    {
        bool HasMoreResults { get; }

        Task<IEnumerable<TDocument>> ReadNext<TDocument>(CancellationToken cancellationToken = default) where TDocument : IIdentifiable;
    }
}