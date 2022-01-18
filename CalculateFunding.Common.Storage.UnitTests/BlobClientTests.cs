using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Storage.UnitTests
{
    [TestClass]
    public class BlobClientTests
    {
        private BlobClient _blobClient;
        private Mock<ICloudBlob> _cloudBlob;
        private Mock<IBlobContainerRepository> _blobContainerRepository;
        
        [TestInitialize]
        public void Setup()
        {
            _blobContainerRepository = new Mock<IBlobContainerRepository>();
            _cloudBlob = new Mock<ICloudBlob>();
            _blobClient = new BlobClient(_blobContainerRepository.Object);
        }

        [TestMethod]
        public void ListBlobs_PrefixPassed_BlobItemsReceived()
        {
            string prefix = NewRandomString();
            string containerName = NewRandomString();
            bool useFlatBlobListing = false;
            BlobListingDetails blobListingDetails = BlobListingDetails.None;

            List<IListBlobItem> listBlobItems = new List<IListBlobItem>
            {
                _cloudBlob.Object
            };

            _blobContainerRepository
                .Setup(_ => _.ListBlobs(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<BlobListingDetails>(), It.IsAny<string>()))
                .Returns(listBlobItems);

            IEnumerable<IListBlobItem> result = _blobClient.ListBlobs(prefix, containerName, useFlatBlobListing, blobListingDetails);

            result.Count().Should().Be(1);
        }

        [TestMethod]
        public async Task BlobExistsAsync_BlobNameGiven_BlobExists()
        {
            string blobName = NewRandomString();

            _blobContainerRepository
                .Setup(_ => _.BlobExistsAsync(blobName, It.IsAny<string>()))
                .ReturnsAsync(true);

            bool exists = await _blobClient.BlobExistsAsync(blobName);

            exists
                .Should()
                .Be(true);
        }

        [TestMethod]
        public async Task GetAsync_BlobNameGiven_ReturnsStream()
        {
            string blobName = NewRandomString();
            MemoryStream memoryStream = new MemoryStream(0);

            _cloudBlob.Setup(_ => _.OpenReadAsync(null, null, null))
                .ReturnsAsync(memoryStream);

            _blobContainerRepository
                .Setup(_ => _.GetBlobReferenceFromServerAsync(blobName, It.IsAny<string>()))
                .ReturnsAsync(_cloudBlob.Object);

            Stream stream = await _blobClient.GetAsync(blobName);

            _blobContainerRepository.Verify(_ => _.GetBlobReferenceFromServerAsync(blobName, It.IsAny<string>()));

            stream
                .Should()
                .Be(memoryStream);
        }

        [TestMethod]
        public async Task UploadFileAsync_CloudBlobGiven_UpoadsFile()
        {
            string blobName = NewRandomString();
            MemoryStream memoryStream = new MemoryStream(0);

            await _blobClient.UploadFileAsync(_cloudBlob.Object, memoryStream);

            _cloudBlob.Verify(_ => _.UploadFromStreamAsync(memoryStream));
        }

        [TestMethod]
        public async Task IsHealthOk_GivenHealthy_ReturnsTrue()
        {
            _blobContainerRepository.Setup(_ => _.VerifyContainer())
                .Verifiable();

            (bool Healthy, string Message) response = await _blobClient.IsHealthOk();

            response.Healthy.Should().Be(true);
        }

        [TestMethod]
        public async Task IsHealthOk_GivenUnHealthy_ReturnsFalse()
        {
            string message = "UnHealthy";

            _blobContainerRepository.Setup(_ => _.VerifyContainer())
                .Throws(new Exception(message));

            (bool Healthy, string Message) response = await _blobClient.IsHealthOk();

            response.Healthy.Should().Be(false);
            response.Message.Should().Be(message);
        }

        [TestMethod]
        public async Task BatchProcessBlobs_GivenListItems_BatchProcessed()
        {
            BlobContinuationToken blobContinuationToken = new BlobContinuationToken();

            List<IListBlobItem> listBlobItems = new List<IListBlobItem>
            {
                _cloudBlob.Object,
                _cloudBlob.Object
            };

            _blobContainerRepository.Setup(_ => _.BatchProcessBlobs(1, It.IsAny<string>(), null))
                .ReturnsAsync((blobContinuationToken, new[] { listBlobItems[0] }));

            _blobContainerRepository.Setup(_ => _.BatchProcessBlobs(1, It.IsAny<string>(), blobContinuationToken))
                .ReturnsAsync((null, new[] { listBlobItems[1] }));

            await _blobClient.BatchProcessBlobs(blobs => { return Task.CompletedTask; }, batchSize: 1);
        }

        [TestMethod]
        public void GetBlobSasUrl_CloudBlobName_SasUrlReturned()
        {
            string blobName = NewRandomString();
            string sharedAccessKey = NewRandomString();
            string uri = NewRandomUrl();

            _blobContainerRepository.Setup(_ => _.GetBlockBlobReference(blobName, It.IsAny<string>()))
                .Returns(_cloudBlob.Object);

            _cloudBlob.Setup(_ => _.GetSharedAccessSignature(It.IsAny<SharedAccessBlobPolicy>()))
                .Returns(sharedAccessKey);

            _cloudBlob.Setup(_ => _.Uri).Returns(new Uri(uri));

            string sasUrl = _blobClient.GetBlobSasUrl(blobName, new DateTimeOffset(), new SharedAccessBlobPermissions());

            sasUrl.Should()
                .Be($"{uri}{sharedAccessKey}");
        }

        [TestMethod]
        public async Task UploadFileAsync_CloudBlobNameAndContents_UpoadsFile()
        {
            string blobName = NewRandomString();
            DateTime filecontents = DateTime.UtcNow;

            Mock<CloudBlockBlob> cloudBlobClient = new Mock<CloudBlockBlob>(new object[] { new Uri(NewRandomUrl()) });

            _blobContainerRepository.Setup(_ => _.GetBlockBlobReference(blobName, It.IsAny<string>()))
                .Returns(cloudBlobClient.Object);

            await _blobClient.UploadFileAsync(blobName, filecontents, It.IsAny<string>());

            cloudBlobClient.Verify(_ => _.UploadTextAsync(JsonConvert.SerializeObject(filecontents)));
        }

        [TestMethod]
        public async Task UploadFileAsync_CloudBlobNameAndStringContents_UpoadsFile()
        {
            string blobName = NewRandomString();
            string filecontents = NewRandomString();

            Mock<CloudBlockBlob> cloudBlobClient = new Mock<CloudBlockBlob>(new object[] { new Uri(NewRandomUrl()) });

            _blobContainerRepository.Setup(_ => _.GetBlockBlobReference(blobName, It.IsAny<string>()))
                .Returns(cloudBlobClient.Object);

            await _blobClient.UploadFileAsync(blobName, filecontents, It.IsAny<string>());

            cloudBlobClient.Verify(_ => _.UploadTextAsync(filecontents));
        }

        [TestMethod]
        public async Task DownFileAsync_CloudBlobGiven_DownloadsFile()
        {
            string blobName = NewRandomString();
            string filecontents = NewRandomString();

            _cloudBlob.Setup(_ => _.DownloadToStreamAsync(It.IsAny<Stream>()))
                .Verifiable();

            Stream stream = await _blobClient.DownloadToStreamAsync(_cloudBlob.Object);

            stream.Position.Should().Be(0);
        }

        [TestMethod]
        public async Task DownloadAsync_CloudBlobNameAndContents_DownloadsFile()
        {
            string blobName = NewRandomString();
            string filecontents = JsonConvert.SerializeObject(string.Empty);

            _blobContainerRepository.Setup(_ => _.DownloadTextAsync(blobName, It.IsAny<string>()))
                .ReturnsAsync(filecontents)
                .Verifiable();

            await _blobClient.DownloadAsync<string>(blobName, It.IsAny<string>());
        }

        [TestMethod]
        public async Task AddMetadataAsync_CloudBlobGiven_MetaDataSet()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string> { { "meta_property", "meta_value" } };

            _cloudBlob.Setup(_ => _.Metadata).Returns(new Dictionary<string, string>());

            await _blobClient.AddMetadataAsync(_cloudBlob.Object, dictionary);

            _cloudBlob.Verify(_ => _.SetMetadataAsync());

            _cloudBlob.Object
                .Metadata
                .Should()
                .BeEquivalentTo(dictionary);
        }

        [TestMethod]
        public async Task StartCopyFromUriAsync_Copie_Blob()
        {
            string sourceContainer = NewRandomString();
            string targetContainer = NewRandomString();
            string sourceBlobName = NewRandomString();
            string targetBlobName = NewRandomString();

            _blobContainerRepository.Setup(_ => _.StartCopyFromUriAsync(sourceContainer, sourceBlobName, targetContainer, targetBlobName))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await _blobClient.StartCopyFromUriAsync(sourceContainer, sourceBlobName, targetContainer, targetBlobName);
        }

        private string NewRandomString() => new RandomString();
        private string NewRandomUrl() => $"http://www.test.com/{new RandomString()}";
    }
}
