using System;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CalculateFunding.Common.Models;
using Microsoft.Azure.Cosmos.Scripts;
using System.IO;
using System.Text.Json;

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
        private IEnumerable<DocumentEntity<CosmosDbTestIdentifiable>> _cosmosDbTestIdentifiableEntities;
        private readonly Reference _aValidReference = new Reference { Id = "testId", Name = "testName"};
        private List<CosmosDbTestIdentifiable> _cosmosDbTestIdentifiables;

        [TestInitialize]
        public void Initialize()
        {
            _databaseName = NewRandomString();
            _containerName = NewRandomString();
            _connectionString = NewRandomString();

            _id = NewRandomString();
            _partitionKey = NewRandomString();
            _cosmosDbTestIdentifiableEntities = new List<DocumentEntity<CosmosDbTestIdentifiable>>
                {new DocumentEntity<CosmosDbTestIdentifiable>() {Content = new CosmosDbTestIdentifiable {Id = _id}}};
            _cosmosDbTestIdentifiables = new List<CosmosDbTestIdentifiable> {new CosmosDbTestIdentifiable {Id = _id}};

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
                .Setup(_ => _.ReadItemAsync<CosmosDbTestIdentifiable>(
                    _id, It.Is<PartitionKey>(p => p == new PartitionKey(_partitionKey)), 
                    It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
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
                .Setup(_ => _.ReadItemAsync<CosmosDbTestIdentifiable>(
                    _id, 
                    It.Is<PartitionKey>(p => p == new PartitionKey(_partitionKey)), 
                    It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .Throws(cosmosException);

            CosmosDbTestIdentifiable response = 
                await _cosmosRepository.TryReadByIdPartitionedAsync<CosmosDbTestIdentifiable>(_id, _partitionKey);

            response.Should().BeNull();
        }

        [TestMethod]
        public async Task CreateItemAsync_CreatesAnItem()
        {
            Mock<ItemResponse<DocumentEntity<CosmosDbTestIdentifiable>>> mockResponse = 
                SetupIdentifiableEntityResponse();
            _container.Setup(c => c.CreateItemAsync(
                    It.IsAny<DocumentEntity<CosmosDbTestIdentifiable>>(),
                    null, It.IsAny<ItemRequestOptions>(), default))
                .ReturnsAsync(mockResponse.Object);

            await _cosmosRepository.CreateAsync(_cosmosDbTestIdentifiables[0], _partitionKey);

            _container.Verify(c=>c.CreateItemAsync(
                It.Is<DocumentEntity<CosmosDbTestIdentifiable>>(doc=>doc.Id == _id),
                null, It.Is<ItemRequestOptions>(itemRequestOptions=> 
                    itemRequestOptions.EnableContentResponseOnWrite == false), default), Times.Once());
        }

        [TestMethod]
        public void BulkUpsertAsync_ThrowsAnException_GivenMoreThanOneMatchingRecordIsFoundInRepository()
        {
            Mock<ItemResponse<DocumentEntity<CosmosDbTestIdentifiable>>> mockResponse = 
                SetupIdentifiableEntityResponse();
            GivenGetItemLinqQueryableReturnsMultipleMatchingRecords();
            GivenUpsertItemAsyncReturnsValidResponse(mockResponse);

            Func<Task> action = async ()=> await _cosmosRepository.BulkUpsertAsync(_cosmosDbTestIdentifiables);

            action.Should().Throw<Exception>().Where(e=>e.Message == "Expected 1 record, found 2, aborting");
        }

        [TestMethod]
        public async Task BulkUpsertAsync_UpsertsAnItemInAzureCosmos_GivenDefaultOptions()
        {
            Mock<ItemResponse<DocumentEntity<CosmosDbTestIdentifiable>>> mockResponse = 
                SetupIdentifiableEntityResponse();
            GivenGetItemLinqQueryableReturnsValidEntitiesWithOneMatchingRecord();
            GivenUpsertItemAsyncReturnsValidResponse(mockResponse);

            await _cosmosRepository.BulkUpsertAsync(_cosmosDbTestIdentifiables);

            _container.Verify(c => c.UpsertItemAsync(
                    It.Is<DocumentEntity<CosmosDbTestIdentifiable>>(doc => 
                        doc.Id == _id 
                        && doc.Deleted == false),
                    null, 
                    It.Is<ItemRequestOptions>(itemRequestOptions=> 
                        itemRequestOptions.EnableContentResponseOnWrite == false), 
                    default),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_SoftDeletesAnItemFromAzureCosmos_GivenHardDeleteIsSetToFalse()
        {
            Mock<ResponseMessage> mockedFeedResponse =
                SetupFeedResponseOfIdentifiableEntity(_cosmosDbTestIdentifiableEntities);
            Mock<FeedIterator> mockedFeedIterator =
                SetupFeedIterator(mockedFeedResponse);
            Mock<ItemResponse<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>> mockResponse =
                SetupItemResponseOfIdentifiableEntity();
            GivenGetItemQueryIteratorReturnsAValidFeedIterator(mockedFeedIterator);
            GivenReplaceItemAsyncReturnsAValidResponse(mockResponse);

            await _cosmosRepository.DeleteAsync<CosmosDbTestIdentifiable>(_id, _partitionKey);

            _container.Verify(c => c.ReplaceItemAsync(
                    It.Is<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>(documentEntity => documentEntity.Deleted 
                        && documentEntity.Id == _id),
                    _id, new PartitionKey(_partitionKey), null, default),
                Times.Once);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        public async Task DeleteAsync_SoftDeletesAnItemFromAzureCosmosWithoutPartitionKey_GivenNoPartitionKey(string partitionKey)
        {
            Mock<ResponseMessage> mockedFeedResponse =
                SetupFeedResponseOfIdentifiableEntity(_cosmosDbTestIdentifiableEntities);
            Mock<FeedIterator> mockedFeedIterator =
                SetupFeedIterator(mockedFeedResponse);
            Mock<ItemResponse<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>> mockResponse =
                SetupItemResponseOfIdentifiableEntity();
            GivenGetItemQueryIteratorReturnsAValidFeedIterator(mockedFeedIterator);
            GivenReplaceItemAsyncReturnsAValidResponse(mockResponse);

            await _cosmosRepository.DeleteAsync<CosmosDbTestIdentifiable>(_id, partitionKey);

            _container.Verify(c => c.ReplaceItemAsync(
                    It.Is<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>(documentEntity => documentEntity.Deleted 
                        && documentEntity.Id == _id),
                    _id, PartitionKey.None, null, default),
                Times.Once);
        }

        [TestMethod]
        public async Task DeleteAsync_HardDeletesAnItemFromAzureCosmos_GivenHardDeleteIsSetToTrue()
        {
            Mock<ResponseMessage> mockedFeedResponse =
                SetupFeedResponseOfIdentifiableEntity(_cosmosDbTestIdentifiableEntities);
            Mock<FeedIterator> mockedFeedIterator =
                SetupFeedIterator(mockedFeedResponse);
            Mock<ItemResponse<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>> mockResponse =
                SetupItemResponseOfIdentifiableEntity();
            GivenGetItemQueryIteratorReturnsAValidFeedIterator(mockedFeedIterator);
            GivenDeleteItemAsyncReturnsAValidResponse(mockResponse);

            await _cosmosRepository.DeleteAsync<CosmosDbTestIdentifiable>(_id, _partitionKey, true);

            _container.Verify(c => c.DeleteItemAsync<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>(
                    _id, new PartitionKey(_partitionKey), null, default),
                Times.Once);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(null)]
        public async Task DeleteAsync_HardDeletesAnItemFromAzureCosmosWithoutPartitionKey_GivenNoPartitionKey(string partitionKey)
        {
            Mock<ResponseMessage> mockedFeedResponse =
                SetupFeedResponseOfIdentifiableEntity(_cosmosDbTestIdentifiableEntities);
            Mock<FeedIterator> mockedFeedIterator =
                SetupFeedIterator(mockedFeedResponse);
            Mock<ItemResponse<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>> mockResponse =
                SetupItemResponseOfIdentifiableEntity();
            GivenGetItemQueryIteratorReturnsAValidFeedIterator(mockedFeedIterator);
            GivenDeleteItemAsyncReturnsAValidResponse(mockResponse);

            await _cosmosRepository.DeleteAsync<CosmosDbTestIdentifiable>(_id, partitionKey, true);

            _container.Verify(c => c.DeleteItemAsync<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>(
                    _id, PartitionKey.None, null, default),
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_ReplacesAnItemInAzureCosmosWithTheItemDeletedSetToFalse_GivenUndeletedIsSetToFalse()
        {
            Mock<ItemResponse<DocumentEntity<Reference>>> mockResponse = SetupItemResponseOfReferenceEntity();
            GivenReplaceItemAsyncReturnsAValidResponse(mockResponse);

            await _cosmosRepository.UpdateAsync(_aValidReference);

            _container.Verify(c=>c.ReplaceItemAsync(
                    It.Is<DocumentEntity<Reference>>(d=>d.Deleted == false), 
                    _aValidReference.Id, 
                    null, 
                    It.IsAny<ItemRequestOptions>(), 
                    default), 
                Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_ReplacesAnItemInAzureCosmosWithTheItemDeletedSetToFalse_GivenUndeletedIsSetToTrue()
        {
            Mock<ItemResponse<DocumentEntity<Reference>>> mockResponse = SetupItemResponseOfReferenceEntity();
            GivenReplaceItemAsyncReturnsAValidResponse(mockResponse);

            await _cosmosRepository.UpdateAsync(_aValidReference, true);

            _container.Verify(c=>c.ReplaceItemAsync(
                    It.Is<DocumentEntity<Reference>>(d=>d.Deleted == false), 
                    _aValidReference.Id, 
                    null, 
                    It.IsAny<ItemRequestOptions>(), 
                    default), 
                Times.Once);
        }

        [TestMethod]
        public async Task BulkUpdateAsync_ExecutesStoredProcedure_GivenStoredProcedureName()
        {
            const string aValidStoredProcedureName = "aTestStoredProcedureName";
            Mock<StoredProcedureExecuteResponse<string>> mockStoredProcedureExecuteResponse =
                new Mock<StoredProcedureExecuteResponse<string>>();
            mockStoredProcedureExecuteResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Accepted);
            Mock<Scripts> mockScripts = new Mock<Scripts>();
            mockScripts.Setup(s =>
                    s.ExecuteStoredProcedureAsync<string>(aValidStoredProcedureName, PartitionKey.Null, It.IsAny<dynamic[]>(),
                        null, default))
                .ReturnsAsync(mockStoredProcedureExecuteResponse.Object);
            _container.Setup(c => c.Scripts).Returns(mockScripts.Object);

            await _cosmosRepository.BulkUpdateAsync(_cosmosDbTestIdentifiables, aValidStoredProcedureName);

            mockScripts.Verify(s => s.ExecuteStoredProcedureAsync<string>(
                    aValidStoredProcedureName, PartitionKey.Null, It.IsAny<dynamic[]>(), null, default),
                Times.Once);
        }

        private void GivenUpsertItemAsyncReturnsValidResponse(Mock<ItemResponse<DocumentEntity<CosmosDbTestIdentifiable>>> mockResponse) =>
            _container.Setup(c => c.UpsertItemAsync(It.IsAny<DocumentEntity<CosmosDbTestIdentifiable>>(), null,
                    It.IsAny<ItemRequestOptions>(), default))
                .ReturnsAsync(mockResponse.Object);

        private void GivenGetItemLinqQueryableReturnsValidEntitiesWithOneMatchingRecord() =>
            _container.Setup(c => c.GetItemLinqQueryable<DocumentEntity<CosmosDbTestIdentifiable>>(
                    true,
                    null,
                    It.IsAny<QueryRequestOptions>(),
                    null))
                .Returns(new EnumerableQuery<DocumentEntity<CosmosDbTestIdentifiable>>(_cosmosDbTestIdentifiableEntities));

        private void GivenGetItemLinqQueryableReturnsMultipleMatchingRecords() =>
            _container.Setup(c => c.GetItemLinqQueryable<DocumentEntity<CosmosDbTestIdentifiable>>(
                    true,
                    null,
                    It.IsAny<QueryRequestOptions>(),
                    null))
                .Returns(new EnumerableQuery<DocumentEntity<CosmosDbTestIdentifiable>>(
                    new List<DocumentEntity<CosmosDbTestIdentifiable>>
                    {
                        new DocumentEntity<CosmosDbTestIdentifiable>()
                            {Content = new CosmosDbTestIdentifiable {Id = _id}},
                        new DocumentEntity<CosmosDbTestIdentifiable>()
                            {Content = new CosmosDbTestIdentifiable {Id = _id}}
                    }));

        private static Mock<ItemResponse<DocumentEntity<CosmosDbTestIdentifiable>>> SetupIdentifiableEntityResponse()
        {
            Mock<ItemResponse<DocumentEntity<CosmosDbTestIdentifiable>>> mockResponse =
                new Mock<ItemResponse<DocumentEntity<CosmosDbTestIdentifiable>>>();
            mockResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Accepted);
            return mockResponse;
        }

        private void GivenReplaceItemAsyncReturnsAValidResponse(Mock<ItemResponse<DocumentEntity<Reference>>> mockResponse) =>
            _container.Setup(c => c.ReplaceItemAsync(
                    It.IsAny<DocumentEntity<Reference>>(),
                    _aValidReference.Id,
                    null,
                    It.IsAny<ItemRequestOptions>(),
                    default))
                .ReturnsAsync(mockResponse.Object);

        private static Mock<ItemResponse<DocumentEntity<Reference>>> SetupItemResponseOfReferenceEntity()
        {
            Mock<ItemResponse<DocumentEntity<Reference>>> mockResponse = new Mock<ItemResponse<DocumentEntity<Reference>>>();
            mockResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Accepted);
            return mockResponse;
        }

        private void GivenDeleteItemAsyncReturnsAValidResponse(Mock<ItemResponse<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>> mockResponse) =>
            _container.Setup(c => c.DeleteItemAsync<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>(
                    _id,
                    It.IsAny<PartitionKey>(),
                    null,
                    default))
                .ReturnsAsync(mockResponse.Object);

        private void GivenReplaceItemAsyncReturnsAValidResponse(Mock<ItemResponse<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>> mockResponse) =>
            _container.Setup(c => c.ReplaceItemAsync(
                    It.IsAny<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>(),
                    It.IsAny<string>(),
                    It.IsAny<PartitionKey>(), null, default))
                .ReturnsAsync(mockResponse.Object);

        private static Mock<ItemResponse<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>> SetupItemResponseOfIdentifiableEntity()
        {
            Mock<ItemResponse<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>> mockResponse =
                new Mock<ItemResponse<DocumentEntity<DocumentEntity<CosmosDbTestIdentifiable>>>>();
            mockResponse.Setup(s => s.StatusCode).Returns(HttpStatusCode.Accepted);
            return mockResponse;
        }

        private void GivenGetItemQueryIteratorReturnsAValidFeedIterator(
            Mock<FeedIterator> mockedFeedIterator) =>
            _container.Setup(c => c.GetItemQueryStreamIterator(
                    It.IsAny<QueryDefinition>(), null, null))
                .Returns(mockedFeedIterator.Object);

        private static Mock<FeedIterator> SetupFeedIterator(
            Mock<ResponseMessage> mockedFeedResponse)
        {
            Mock<FeedIterator> mockedFeedIterator =
                new Mock<FeedIterator>();
            mockedFeedIterator.SetupSequence(c => c.HasMoreResults).Returns(true).Returns(false);
            mockedFeedIterator.Setup(c => c.ReadNextAsync(default)).ReturnsAsync(
                mockedFeedResponse.Object);
            return mockedFeedIterator;
        }

        private static Mock<ResponseMessage> SetupFeedResponseOfIdentifiableEntity(
            IEnumerable<DocumentEntity<CosmosDbTestIdentifiable>> items)
        {
            Mock<ResponseMessage> mockedFeedResponse =
                new Mock<ResponseMessage>();

            MemoryStream stream = new MemoryStream(
                System.Text.Encoding.UTF8.GetBytes(
                    JsonSerializer.Serialize(
                        new QueryStream<DocumentEntity<CosmosDbTestIdentifiable>> 
                        {  
                            Documents = items 
                        }
                    )
                )
            );

            mockedFeedResponse.Setup(f => f.Content).Returns(stream);
            return mockedFeedResponse;
        }

        private string NewRandomString() => new RandomString();

        private int NewRandomInteger() => new RandomNumberBetween(0, 100);
    }
}
