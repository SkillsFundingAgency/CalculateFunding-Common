namespace CalculateFunding.Common.CosmosDb
{
    public class CosmosDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string ContainerName { get; set; }
        public string PartitionKey { get; set; }

    }
}