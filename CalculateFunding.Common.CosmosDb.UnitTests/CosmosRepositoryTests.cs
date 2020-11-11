using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CalculateFunding.Common.CosmosDb.UnitTests
{
    [TestClass]
    public class CosmosRepositoryTests
    {
        private Mock<Database> _database;
        private Mock<Container> _container;

        private Mock<CosmosClient> _cosmosDbClient;
        
        private string _databaseName;
        private string _containerName;
        private string _connectionString;

        private string _id;
        private string _partitionKey;

        private CosmosRepositoryTest _cosmosRepository;

        [TestInitialize]
        public void Initialize()
        {
            _databaseName = NewRandomString();
            _containerName = NewRandomString();
            _connectionString = NewRandomString();

            _id = NewRandomString();
            _partitionKey = NewRandomString();

            _database = new Mock<Database>();
            _container = new Mock<Container>();

            _cosmosDbClient = new Mock<CosmosClient>();
            _cosmosDbClient
                .Setup(_ => _.GetDatabase(_databaseName))
                .Returns(_database.Object);

            _database
                .Setup(_ => _.GetContainer(_containerName))
                .Returns(_container.Object);

            _cosmosRepository = new CosmosRepositoryTest(
                new CosmosDbSettings {
                    ContainerName = _containerName,
                    DatabaseName = _databaseName,
                    ConnectionString = _connectionString
                }, 
                _cosmosDbClient.Object);
        }

        [TestMethod]
        public async Task TryReadByIdPartitionedAsync_SuccessCall_CallsContainerReadItemAsync()
        {
            CosmosDbTestIdentifiable item = new CosmosDbTestIdentifiable { Id = _id };
            Mock<ItemResponse<CosmosDbTestIdentifiable>> itemResponse = new Mock<ItemResponse<CosmosDbTestIdentifiable>>();
            itemResponse.Setup(_ => _.Resource).Returns(item);

            _container
                .Setup(_ => _.ReadItemAsync<CosmosDbTestIdentifiable>(_id, It.Is<PartitionKey>(p => p == new PartitionKey(_partitionKey)), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(itemResponse.Object);

            CosmosDbTestIdentifiable response = 
                await _cosmosRepository.TryReadByIdPartitionedAsync<CosmosDbTestIdentifiable>(_id, _partitionKey);

            response.Should().NotBeNull();
            response.Id.Should().Be(_id);
        }

        [TestMethod]
        public async Task TryReadByIdPartitionedAsync_ThrowsCosmosException_ReturnsDefaultValue()
        {
            CosmosException cosmosException = new CosmosException(
                NewRandomString(),
                HttpStatusCode.NotFound,
                NewRandomInteger(),
                NewRandomString(),
                NewRandomInteger());

            _container
                .Setup(_ => _.ReadItemAsync<CosmosDbTestIdentifiable>(_id, It.Is<PartitionKey>(p => p == new PartitionKey(_partitionKey)), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .Throws(cosmosException);

            CosmosDbTestIdentifiable response = 
                await _cosmosRepository.TryReadByIdPartitionedAsync<CosmosDbTestIdentifiable>(_id, _partitionKey);

            response.Should().BeNull();
        }

        private string NewRandomString() => new RandomString();
        private int NewRandomInteger() => new RandomNumberBetween(0, 100);
    }
}
