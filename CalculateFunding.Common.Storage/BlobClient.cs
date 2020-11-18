using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.Utility;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;

namespace CalculateFunding.Common.Storage
{
    public class BlobClient : IBlobClient
    {
        private readonly IBlobContainerRepository _blobContainerRepository;

        public BlobClient(IBlobContainerRepository blobContainerRepository)
        {
            Guard.ArgumentNotNull(blobContainerRepository, nameof(blobContainerRepository));
            
            _blobContainerRepository = blobContainerRepository;
        }

        public async Task<(bool Ok, string Message)> IsHealthOk()
        {
            try
            {
                _blobContainerRepository.VerifyContainer(); 
                return await Task.FromResult((true, string.Empty));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task BatchProcessBlobs(Func<IEnumerable<IListBlobItem>, Task> batchProcessor, 
            string containerName = null, 
            int batchSize = 50)
        {
            (BlobContinuationToken BlobContinuationToken, IEnumerable<IListBlobItem> Results) response = (null, null);

            do
            {
                response = await _blobContainerRepository.BatchProcessBlobs(batchSize, continuationToken: response.BlobContinuationToken, containerName: containerName);

                await batchProcessor(response.Results);

            } while (response.BlobContinuationToken != null);
        }

        public void VerifyFileName(string fileName)
        {
            Guard.IsNullOrWhiteSpace(fileName, nameof(fileName));

            if (fileName.Length > 254)
            {
                throw new ArgumentException("Storage File name is too long, max length is 254 characters", nameof(fileName));
            }

            if (fileName.EndsWith(".", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Storage File name cannot end with a period (.)", nameof(fileName));
            }

            if (fileName.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Storage File name cannot end with a forward slash (/)", nameof(fileName));
            }
        }

        public async Task<bool> DoesBlobExistAsync(string blobName, string containerName = null)
        {
            Guard.IsNullOrWhiteSpace(blobName, nameof(blobName));

            return await _blobContainerRepository.BlobExistsAsync(blobName, containerName);
        }

        public string GetBlobSasUrl(string blobName,
            DateTimeOffset finish,
            SharedAccessBlobPermissions permissions, string containerName = null)
        {
            ICloudBlob blob = GetBlockBlobReference(blobName, containerName);
            
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = finish,
                Permissions = permissions
            };

            string sharedAccessSignature = blob.GetSharedAccessSignature(sasConstraints);

            return $"{blob.Uri}{sharedAccessSignature}";
        }

        public async Task<T> DownloadAsync<T>(string blobName, string containerName = null)
        {
            Guard.IsNullOrWhiteSpace(blobName, nameof(blobName));

            string json = await _blobContainerRepository.DownloadTextAsync(blobName, containerName);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<string> UploadFileAsync<T>(string blobName, T contents, string containerName = null)
        {
            Guard.ArgumentNotNull(contents, nameof(contents));

            string json = JsonConvert.SerializeObject(contents);

            return await UploadFileAsync(blobName, json, containerName);
        }

        public async Task<Stream> DownloadToStreamAsync(ICloudBlob blob)
        {
            Guard.ArgumentNotNull(blob, nameof(blob));

            MemoryStream stream = new MemoryStream();
            await blob.DownloadToStreamAsync(stream);
            stream.Position = 0;

            return stream;
        }

        public async Task<string> UploadFileAsync(string blobName, string fileContents, string containerName = null)
        {
            Guard.IsNullOrWhiteSpace(blobName, nameof(blobName));

            VerifyFileName(blobName);

            CloudBlockBlob blob = (CloudBlockBlob)GetBlockBlobReference(blobName, containerName);

            await blob.UploadTextAsync(fileContents);

            return blob.Uri.ToString();
        }

        public async Task<bool> BlobExistsAsync(string blobName, string containerName = null)
        {
            return await DoesBlobExistAsync(blobName, containerName);
        }

        public async Task<Stream> GetAsync(string blobName, string containerName = null)
        {
            ICloudBlob blob = await GetBlobReferenceFromServerAsync(blobName, containerName);

            return await blob.OpenReadAsync(null, null, null);
        }

        public IEnumerable<IListBlobItem> ListBlobs(string prefix = null, string containerName = null, bool useFlatBlobListing = false, BlobListingDetails blobListingDetails = BlobListingDetails.None)
        {
            return _blobContainerRepository.ListBlobs(prefix, useFlatBlobListing, blobListingDetails, containerName);
        }

        public async Task UploadFileAsync(ICloudBlob blob, Stream data)
        {
            //reset to start to handle resilience policy retrying on a single stream instance
            data.Position = 0;

            await blob.UploadFromStreamAsync(data);
        }

        public async Task AddMetadataAsync(ICloudBlob blob, IDictionary<string, string> metadata)
        {
            foreach (KeyValuePair<string, string> metadataItem in metadata.Where(_ => !string.IsNullOrEmpty(_.Value)))
            {
                blob.Metadata.Add(
                    ReplaceInvalidMetadataKeyCharacters(metadataItem.Key),
                    metadataItem.Value);
            }

            await blob.SetMetadataAsync();
        }

        private string ReplaceInvalidMetadataKeyCharacters(string metadataKey)
        {
            return metadataKey.Replace('-', '_');
        }

        public ICloudBlob GetBlockBlobReference(string blobName, string containerName = null)
        {
            return _blobContainerRepository.GetBlockBlobReference(blobName, containerName);
        }

        public Task<ICloudBlob> GetBlobReferenceFromServerAsync(string blobName, string containerName = null)
        {
            return _blobContainerRepository.GetBlobReferenceFromServerAsync(blobName, containerName);
        }
    }
}
