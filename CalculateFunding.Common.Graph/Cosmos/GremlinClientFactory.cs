using CalculateFunding.Common.Graph.Interfaces;
using CalculateFunding.Common.Utility;
using Gremlin.Net.Driver;
using Gremlin.Net.Structure.IO.GraphSON;
using System;
using System.Net.WebSockets;

namespace CalculateFunding.Common.Graph.Cosmos
{
    public class GremlinClientFactory : IGremlinClientFactory
    {
        private readonly GremlinServer _server;
        private readonly ConnectionPoolSettings _connectionPoolSettings;
        private readonly Action<ClientWebSocketOptions> _webSocketConfiguration;
        private IGremlinClient _gremlinClient;

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
                MaxInProcessPerConnection = settings.MaxInProcessPerConnection ?? 10,
                PoolSize = settings.MaxInProcessPerConnection ?? 30,
                ReconnectionAttempts = settings.ReconnectionAttempts ?? 3,
                ReconnectionBaseDelay = TimeSpan.FromMilliseconds(settings.ReconnectionBaseDelay ?? 500)
            };

            _webSocketConfiguration =
                new Action<ClientWebSocketOptions>(options =>
                {
                    options.KeepAliveInterval = TimeSpan.FromSeconds(settings.KeepAliveInterval ?? 10);
                });
        }
        
        public IGremlinClient CreateClient() => _gremlinClient ??
            new GremlinClient(_server,
                new GraphSON2Reader(),
                new GraphSON2Writer(),
                GremlinClient.GraphSON2MimeType,
                _connectionPoolSettings,
                _webSocketConfiguration);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _gremlinClient?.Dispose();
            }
        }
    }
}