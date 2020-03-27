using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Common.Storage.UnitTests
{
    [TestClass]
    public class BlobClientTests
    {
        private BlobClient _blobClient;
        private BlobStorageOptions _azureStorageSettings;
        private Mock<IBlobContainerRepository> _blobContainerRepository;
        private Mock<CloudBlobContainer> _cloudBlobContainer;

        [TestInitialize]
        public void Setup()
        {
            _azureStorageSettings = new BlobStorageOptions
            { 
                ConnectionString = NewRandomString(), 
                ContainerName = NewRandomString() 
            };

            _cloudBlobContainer = new Mock<CloudBlobContainer>(MockBehavior.Strict, new object[] { new Uri(NewRandomUrl()) });

            _blobContainerRepository = new Mock<IBlobContainerRepository>();

            _blobContainerRepository
                .Setup(_ => _.GetCloudBlobContainer(It.IsAny<BlobStorageOptions>(), It.IsAny<string>()))
                .Returns(_cloudBlobContainer.Object);

            _blobClient = new BlobClient(_azureStorageSettings, _blobContainerRepository.Object);
        }

        [TestMethod]
        public void ListBlobs_PrefixPassed_BlobItemsReceived()
        {
            string prefix = NewRandomString();
            string containerName = NewRandomString();
            bool useFlatBlobListing = false;
            BlobListingDetails blobListingDetails = BlobListingDetails.None;

            Mock<ICloudBlob> cloudBlob = new Mock<ICloudBlob>();

            List<IListBlobItem> listBlobItems = new List<IListBlobItem>
            {
                cloudBlob.Object
            };

            _cloudBlobContainer
                .Setup(_ => _.ListBlobs(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<BlobListingDetails>(), It.IsAny<BlobRequestOptions>(), It.IsAny<OperationContext>()))
                .Returns(listBlobItems);

            IEnumerable<IListBlobItem> result = _blobClient.ListBlobs(prefix, containerName, useFlatBlobListing, blobListingDetails);

            result.Count().Should().Be(1);

            _cloudBlobContainer
                .Verify(_ => _.ListBlobs(prefix, useFlatBlobListing, blobListingDetails, It.IsAny<BlobRequestOptions>(), It.IsAny<OperationContext>()), Times.Once());

            _blobContainerRepository
                .Verify(_ => _.GetCloudBlobContainer(It.IsAny<BlobStorageOptions>(), containerName), Times.Once());
        }

        private string NewRandomString() => new RandomString();
        private string NewRandomUrl() => $"http://www.test.com/{new RandomString()}";
    }
}
