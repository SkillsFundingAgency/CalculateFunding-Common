using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Utility;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Storage
{
    public class BlobContainerRepository : IBlobContainerRepository
    {
        private readonly BlobStorageOptions _azureStorageSettings;
        private static readonly ConcurrentDictionary<string, CloudBlobContainer> _containers = new ConcurrentDictionary<string, CloudBlobContainer>();

        public BlobContainerRepository(BlobStorageOptions blobStorageSettings)
        {
            Guard.ArgumentNotNull(blobStorageSettings, nameof(blobStorageSettings));
            Guard.IsNullOrWhiteSpace(blobStorageSettings.ConnectionString, nameof(blobStorageSettings.ConnectionString));
            Guard.IsNullOrWhiteSpace(blobStorageSettings.ContainerName, nameof(blobStorageSettings.ContainerName));
            
            _azureStorageSettings = blobStorageSettings;
        }

        private CloudBlobContainer GetContainer(string containerName = null)
        {
            containerName ??= _azureStorageSettings.ContainerName;

            return _containers.GetOrAdd(containerName.ToLower(), (key) =>
            {
                CloudBlobContainer container = GetOrCreateContainer(containerName.ToLower());

                container.CreateIfNotExists();

                return container;
            });
        }

        public virtual CloudBlobContainer GetOrCreateContainer(string containerName = null)
        {
            CloudStorageAccount credentials = CloudStorageAccount.Parse(_azureStorageSettings.ConnectionString);
            CloudBlobClient client = credentials.CreateCloudBlobClient();
            return client.GetContainerReference(containerName?.ToLower() ?? _azureStorageSettings.ContainerName.ToLower());
        }

        public void VerifyContainer()
        {
            GetContainer();
        }

        public ICloudBlob GetBlockBlobReference(string blobName, string containerName = null)
        {
            Guard.IsNullOrWhiteSpace(blobName, nameof(blobName));

            return GetContainer(containerName).GetBlockBlobReference(blobName);
        }

        public Task<ICloudBlob> GetBlobReferenceFromServerAsync(string blobName, string containerName = null)
        {
            Guard.IsNullOrWhiteSpace(blobName, nameof(blobName));

            return GetContainer(containerName).GetBlobReferenceFromServerAsync(blobName);
        }

        public IEnumerable<IListBlobItem> ListBlobs(string prefix, bool useFlatBlobListing, BlobListingDetails blobListingDetails, string containerName = null)
        {
            CloudBlobContainer container = GetContainer(containerName);

            return container.ListBlobs(prefix, useFlatBlobListing, blobListingDetails);
        }

        public async Task<bool> BlobExistsAsync(string blobName, string containerName = null)
        {
            Guard.IsNullOrWhiteSpace(blobName, nameof(blobName));
            
            ICloudBlob blob = GetContainer(containerName).GetBlockBlobReference(blobName);

            return await blob.ExistsAsync();
        }

        public async Task<string> DownloadTextAsync(string blobName, string containerName = null)
        {
            CloudBlockBlob blob = GetContainer(containerName).GetBlockBlobReference(blobName);
            return await blob.DownloadTextAsync();
        }

        public async Task<(BlobContinuationToken, IEnumerable<IListBlobItem>)> BatchProcessBlobs(int batchSize, string containerName = null, BlobContinuationToken continuationToken = null)
        {
            CloudBlobContainer container = GetContainer(containerName);

            BlobResultSegment response = await container.ListBlobsSegmentedAsync(
                    prefix: null,
                    useFlatBlobListing: true,
                    currentToken: continuationToken,
                    maxResults: batchSize,
                    options: null,
                    operationContext: null,
                    blobListingDetails: BlobListingDetails.None);

            return (response.ContinuationToken, response.Results);
        }

        public async Task StartCopyFromUriAsync(string sourceContainer, string sourceBlobName, string targetContainer, string targetBlobName)
        {
            CloudBlockBlob source = GetContainer(sourceContainer)
                .GetBlockBlobReference(sourceBlobName);

            CloudBlockBlob target = GetContainer(targetContainer)
                .GetBlockBlobReference(targetBlobName);

            await target.StartCopyAsync(source);
        }
    }
}
