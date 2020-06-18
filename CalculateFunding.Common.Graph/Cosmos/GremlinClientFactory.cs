using CalculateFunding.Common.Graph.Interfaces;
using CalculateFunding.Common.Utility;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;

namespace CalculateFunding.Common.Graph.Cosmos
{
    public class GremlinClientFactory : IGremlinClientFactory
    {
        private readonly GremlinServer _server;         
        
        public GremlinClientFactory(ICosmosGraphDbSettings settings)
        {
            Guard.ArgumentNotNull(settings, nameof(settings));
            Guard.IsNullOrWhiteSpace(settings.EndPointUrl, nameof(settings.EndPointUrl));
            Guard.IsNullOrWhiteSpace(settings.ApiKey, nameof(settings.ApiKey));
            Guard.IsNullOrWhiteSpace(settings.ContainerPath, nameof(settings.ContainerPath));

            _server = new GremlinServer(settings.EndPointUrl,
                settings.Port,
                true,
                settings.ContainerPath,
                settings.ApiKey);
        }
        
        public IGremlinClient CreateClient() =>
            new GremlinClient(_server,
                new GraphSON2Reader(),
                new GraphSON2Writer(),
                GremlinClient.GraphSON2MimeType);
    }
}