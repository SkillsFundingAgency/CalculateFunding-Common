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
        private readonly GremlinServer _server;
        private readonly ConnectionPoolSettings _connectionPoolSettings;
        private readonly Action<ClientWebSocketOptions> _webSocketConfiguration;

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

            _connectionPoolSettings = new ConnectionPoolSettings()
            {
                MaxInProcessPerConnection = settings.MaxInProcessPerConnection ?? 32,
                PoolSize = settings.PoolSize ?? 4,
                ReconnectionAttempts = settings.ReconnectionAttempts ?? 4,
                ReconnectionBaseDelay = TimeSpan.FromMilliseconds(settings.ReconnectionBaseDelay ?? 500)
            };

            _webSocketConfiguration =
                new Action<ClientWebSocketOptions>(options =>
                {
                    options.KeepAliveInterval = TimeSpan.FromSeconds(settings.KeepAliveInterval ?? 10);
                });
        }

        public IGremlinClient Client => new GremlinClient(_server,
                    new GraphSON2Reader(),
                    new GraphSON2Writer(),
                    GremlinClient.GraphSON2MimeType,
                    _connectionPoolSettings,
                    _webSocketConfiguration);
    }
}