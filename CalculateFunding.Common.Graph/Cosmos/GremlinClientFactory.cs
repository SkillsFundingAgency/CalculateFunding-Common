using CalculateFunding.Common.Graph.Interfaces;
using CalculateFunding.Common.Utility;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using System;
using System.Net.WebSockets;
using System.Threading;

namespace CalculateFunding.Common.Graph.Cosmos
{
    public class GremlinClientFactory : IGremlinClientFactory
    {
        private readonly IGremlinClient _client;

        public GremlinClientFactory(ICosmosGraphDbSettings settings)
        {
            Guard.ArgumentNotNull(settings, nameof(settings));
            Guard.IsNullOrWhiteSpace(settings.EndPointUrl, nameof(settings.EndPointUrl));
            Guard.IsNullOrWhiteSpace(settings.ApiKey, nameof(settings.ApiKey));
            Guard.IsNullOrWhiteSpace(settings.ContainerPath, nameof(settings.ContainerPath));

            GremlinServer server = new GremlinServer(settings.EndPointUrl,
                settings.Port,
                true,
                settings.ContainerPath,
                settings.ApiKey);

            ConnectionPoolSettings connectionPoolSettings = new ConnectionPoolSettings()
            {
                MaxInProcessPerConnection = settings.MaxInProcessPerConnection ?? 128,
                PoolSize = settings.PoolSize ?? 4,
                ReconnectionAttempts = settings.ReconnectionAttempts ?? 4,
                ReconnectionBaseDelay = TimeSpan.FromMilliseconds(settings.ReconnectionBaseDelay ?? 500)
            };

            Action<ClientWebSocketOptions> webSocketConfiguration =
                new Action<ClientWebSocketOptions>(options =>
                {
                    options.KeepAliveInterval = TimeSpan.FromSeconds(settings.KeepAliveInterval ?? 10);
                });

            _client = new GremlinClient(server,
                    new GraphSON2Reader(),
                    new GraphSON2Writer(),
                    GremlinClient.GraphSON2MimeType,
                    connectionPoolSettings,
                    webSocketConfiguration);
        }

        public IGremlinClient Client => _client;
    }
}