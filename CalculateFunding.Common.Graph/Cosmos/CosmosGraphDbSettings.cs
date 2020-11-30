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
    }
}