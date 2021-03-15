using Gremlin.Net.Driver;

namespace CalculateFunding.Common.Graph.Interfaces
{
    public interface IGremlinClientFactory
    {
        IGremlinClient Client { get; }
    }
}