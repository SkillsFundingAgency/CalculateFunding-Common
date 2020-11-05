using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.CosmosDb.UnitTests
{
    public class CosmosDbTestIdentifiable : IIdentifiable
    {
        public string Id { get; set; }
    }
}
