using CalculateFunding.Common.Testing;

namespace CalculateFunding.Common.Graph.UnitTests.Cosmos
{
    internal class ModelWithImmutablePropertiesBuilder : TestEntityBuilder
    {
        private string _id;
        private string _partitionKey;
        private string _name;

        public ModelWithImmutablePropertiesBuilder WithPartitionKey(string partitionKey)
        {
            _partitionKey = partitionKey;

            return this;
        }

        public ModelWithImmutablePropertiesBuilder WithName(string name)
        {
            _name = name;

            return this;
        }
            
        public ModelWithImmutablePropertiesBuilder WithId(string id)
        {
            _id = id;

            return this;
        }
            
        public ModelWithImmutableProperties Build()
        {
            return new ModelWithImmutableProperties
            {
                Id = _id ?? NewRandomString(),
                Name = _name ?? NewRandomString(),
                PartitionKey = _partitionKey ?? NewRandomString()
            };
        }
    }
}