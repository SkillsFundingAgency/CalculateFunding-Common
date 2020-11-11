using Microsoft.Azure.Cosmos;

namespace CalculateFunding.Common.CosmosDb.UnitTests
{
    public class CosmosRepositoryTest : CosmosRepository
    {
        private readonly CosmosClient _cosmosClient;

        public CosmosRepositoryTest(CosmosDbSettings settings, CosmosClient cosmosClient) : base(settings, null)
        {
            _cosmosClient = cosmosClient;
        }

        protected override CosmosClient GetClient(string connectionString, CosmosClientOptions cosmosClientOptions)
        {
            return _cosmosClient;
        }
    }
}
