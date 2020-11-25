using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Storage.UnitTests
{
    [TestClass]
    public class BlobContainerRepositoryTests
    {
        private BlobContainerRepositoryTest _blobContainerRepository;
        private BlobStorageOptions _blobStorageOptions;
        private Mock<CloudBlobContainer> _cloudBlobContainer;
        private Mock<ICloudBlob> _cloudBlob;
        private Mock<CloudBlockBlob> _cloudBlockBlob;

        [TestInitialize]
        public void Setup()
        {
            _blobStorageOptions = new BlobStorageOptions {
                ContainerName = NewRandomString(),
                ConnectionString = NewRandomString()
            };

            _cloudBlob = new Mock<ICloudBlob>();

            _cloudBlockBlob = new Mock<CloudBlockBlob>(new object[] { new Uri(NewRandomUrl()) });

            _cloudBlobContainer = new Mock<CloudBlobContainer>(new object[] { new Uri(NewRandomUrl())});

            _blobContainerRepository = new BlobContainerRepositoryTest(_blobStorageOptions, _cloudBlobContainer.Object);
        }

        [TestMethod]
        public void VerifyContainer_GivenValidContainer_NoErrorThrown()
        {
            GivenContainer();

            _blobContainerRepository.VerifyContainer();
        }

        [TestMethod]
        public void GetBlockBlobReference_GivenBlobName_ReturnsCloudBlob()
        {
            string blobName = NewRandomString();

            GivenContainer();
            GivenCloudBlockBlob(blobName);

            _blobContainerRepository.GetBlockBlobReference(blobName);
        }

        [TestMethod]
        public void GetBlobReferenceFromServerAsync_GivenBlobName_ReturnsCloudBlob()
        {
            string blobName = NewRandomString();

            GivenContainer();
            GivenCloudBlob(blobName);

            _blobContainerRepository.GetBlobReferenceFromServerAsync(blobName);
        }

        [TestMethod]
        public void ListBlobs_GivenValidArguments_ReturnsList()
        {
            string prefix = NewRandomString();

            List<IListBlobItem> listBlobItems = new List<IListBlobItem>
            {
                _cloudBlob.Object,
                _cloudBlob.Object
            };

            GivenContainer();
            GivenBlobList(listBlobItems);

            _blobContainerRepository.ListBlobs(prefix, false, BlobListingDetails.None);
        }

        [TestMethod]
        public async Task BlobExistsAsync_GivenBlobExists_ReturnsTrue()
        {
            string blobName = NewRandomString();

            GivenContainer();
            GivenCloudBlockBlob(blobName);
            _cloudBlockBlob.Setup(_ => _.ExistsAsync()).ReturnsAsync(true).Verifiable();

            bool exists = await _blobContainerRepository.BlobExistsAsync(blobName);

            exists.Should().Be(true);
        }

        [TestMethod]
        public async Task DownloadTextAsync_GivenBlobExists_ReturnsBlobContent()
        {
            string blobName = NewRandomString();
            string expectedBlobContents = NewRandomString();

            GivenContainer();
            GivenCloudBlockBlob(blobName);
            _cloudBlockBlob.Setup(_ => _.DownloadTextAsync()).ReturnsAsync(expectedBlobContents).Verifiable();

            string blobContents = await _blobContainerRepository.DownloadTextAsync(blobName);

            blobContents.Should().Be(expectedBlobContents);
        }

        [TestMethod]
        public async Task BatchProcessBlobs_GivenValidArguments_BatchProcessed()
        {
            List<IListBlobItem> listBlobItems = new List<IListBlobItem>
            {
                _cloudBlob.Object,
                _cloudBlob.Object
            };

            GivenContainer();
            _cloudBlobContainer.Setup(_ => _.ListBlobsSegmentedAsync(null,
                    true,
                    BlobListingDetails.None,
                    50,
                    null,
                    null,
                    null)).ReturnsAsync(new BlobResultSegment(listBlobItems, null)).Verifiable();

            (BlobContinuationToken ContinuationToken, IEnumerable<IListBlobItem> Items) result = await _blobContainerRepository.BatchProcessBlobs(50);

            result.ContinuationToken.Should().Be(null);
            result.Items.Should().BeEquivalentTo(listBlobItems);
        }

        private void GivenContainer() => _cloudBlobContainer.Setup(_ => _.CreateIfNotExists(It.IsAny<BlobRequestOptions>(), It.IsAny<OperationContext>())).Verifiable();

        private void GivenCloudBlob(string blobName) => _cloudBlobContainer.Setup(_ => _.GetBlobReferenceFromServerAsync(blobName)).ReturnsAsync(_cloudBlob.Object).Verifiable();

        private void GivenCloudBlockBlob(string blobName) => _cloudBlobContainer.Setup(_ => _.GetBlockBlobReference(blobName)).Returns(_cloudBlockBlob.Object).Verifiable();

        private void GivenBlobList(List<IListBlobItem> blobItems) => _cloudBlobContainer.Setup(_ => _.ListBlobs(It.IsAny<string>(), false, BlobListingDetails.None, It.IsAny<BlobRequestOptions>(), It.IsAny<OperationContext>())).Returns(blobItems).Verifiable();

        private string NewRandomString() => new RandomString();
        private string NewRandomUrl() => $"http://www.test.com/{new RandomString()}";
    }
}
