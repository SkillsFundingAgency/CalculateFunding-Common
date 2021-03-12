using CalculateFunding.Common.Graph.Interfaces;

namespace CalculateFunding.Common.Graph.Cosmos
{
    public class CosmosGraphDbSettings : ICosmosGraphDbSettings
    {
        public string EndPointUrl { get; set; }
        
        public int Port { get; set; }
        
        public string ApiKey { get; set; }
        
        public string ContainerPath { get; set; }

        public int DegreeOfParallelism { get; set; }

        public int? MaxInProcessPerConnection { get; set; }
        public int? PoolSize { get; set; }
        public int? ReconnectionAttempts { get; set; }
        public int? ReconnectionBaseDelay { get; set; }
        public int? KeepAliveInterval { get; set; }
    }
}