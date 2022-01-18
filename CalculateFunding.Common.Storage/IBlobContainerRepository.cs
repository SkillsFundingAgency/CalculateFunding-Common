using Microsoft.Azure.Storage.Blob;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateFunding.Common.Storage
{
    public interface IBlobContainerRepository
    {
        void VerifyContainer();

        ICloudBlob GetBlockBlobReference(string blobName, string containerName = null);

        Task<ICloudBlob> GetBlobReferenceFromServerAsync(string blobName, string containerName = null);

        IEnumerable<IListBlobItem> ListBlobs(string prefix, bool useFlatBlobListing, BlobListingDetails blobListingDetails, string containerName = null);

        Task<bool> BlobExistsAsync(string blobName, string containerName = null);

        Task<string> DownloadTextAsync(string blobName, string containerName = null);

        Task<(BlobContinuationToken, IEnumerable<IListBlobItem>)> BatchProcessBlobs(int batchSize, string containerName = null, BlobContinuationToken continuationToken = null);

        Task StartCopyFromUriAsync(string sourceContainer, string sourceBlobName, string targetContainer, string targetBlobName);
    }
}
