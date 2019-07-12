using System;
using System.IO;
using System.Threading.Tasks;
using CalculateFunding.Common.Utility;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;

namespace CalculateFunding.Common.Storage
{
    public class BlobClient : IBlobClient
    {
        private Lazy<CloudBlobContainer> _container;
        private readonly BlobStorageOptions _azureStorageSettings;

        public BlobClient(BlobStorageOptions blobStorageSettings)
        {
            Guard.ArgumentNotNull(blobStorageSettings, nameof(blobStorageSettings));
            Guard.IsNullOrWhiteSpace(blobStorageSettings.ConnectionString, nameof(blobStorageSettings.ConnectionString));
            Guard.IsNullOrWhiteSpace(blobStorageSettings.ContainerName, nameof(blobStorageSettings.ContainerName));

            _azureStorageSettings = blobStorageSettings;

            EnsureBlobClient();
        }

        public void Initialize()
        {
            _container = new Lazy<CloudBlobContainer>(() =>
            {
                CloudStorageAccount credentials = CloudStorageAccount.Parse(_azureStorageSettings.ConnectionString);
                CloudBlobClient client = credentials.CreateCloudBlobClient();
                CloudBlobContainer container = client.GetContainerReference(_azureStorageSettings.ContainerName.ToLower());

                container.CreateIfNotExists();

                return container;
            });
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

        public async Task<(bool Ok, string Message)> IsHealthOk()
        {
            try
            {
                EnsureBlobClient();
                CloudBlobContainer container = _container.Value;
                return await Task.FromResult((true, string.Empty));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task<bool> DoesBlobExistAsync(string blobName)
        {
            Guard.IsNullOrWhiteSpace(blobName, nameof(blobName));

            EnsureBlobClient();

            ICloudBlob blob = await _container.Value.GetBlobReferenceFromServerAsync(blobName);

            return await blob.ExistsAsync();
        }

        public string GetBlobSasUrl(string blobName,
            DateTimeOffset finish,
            SharedAccessBlobPermissions permissions)
        {
            ICloudBlob blob = GetBlockBlobReference(blobName);
            
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
            {
                SharedAccessExpiryTime = finish,
                Permissions = permissions
            };

            string sharedAccessSignature = blob.GetSharedAccessSignature(sasConstraints);

            return $"{blob.Uri}{sharedAccessSignature}";
        }

        public async Task<T> DownloadAsync<T>(string blobName)
        {
            Guard.IsNullOrWhiteSpace(blobName, nameof(blobName));

            EnsureBlobClient();

            CloudBlockBlob blob = _container.Value.GetBlockBlobReference(blobName);
            string json = await blob.DownloadTextAsync();

            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task<string> UploadFileAsync<T>(string blobName, T contents)
        {
            Guard.ArgumentNotNull(contents, nameof(contents));

            string json = JsonConvert.SerializeObject(contents);

            return await UploadFileAsync(blobName, json);
        }

        public ICloudBlob GetBlockBlobReference(string blobName)
        {
            Guard.IsNullOrWhiteSpace(blobName, nameof(blobName));

            EnsureBlobClient();
            
            return _container.Value.GetBlockBlobReference(blobName);
        }

        public Task<ICloudBlob> GetBlobReferenceFromServerAsync(string blobName)
        {
            Guard.IsNullOrWhiteSpace(blobName, nameof(blobName));

            EnsureBlobClient();

            return _container.Value.GetBlobReferenceFromServerAsync(blobName);
        }

        public async Task<Stream> DownloadToStreamAsync(ICloudBlob blob)
        {
            Guard.ArgumentNotNull(blob, nameof(blob));

            EnsureBlobClient();

            MemoryStream stream = new MemoryStream();
            await blob.DownloadToStreamAsync(stream);
            stream.Position = 0;

            return stream;
        }

        public async Task<string> UploadFileAsync(string blobName, string fileContents)
        {
            Guard.IsNullOrWhiteSpace(blobName, nameof(blobName));

            VerifyFileName(blobName);

            EnsureBlobClient();

            CloudBlockBlob blob = _container.Value.GetBlockBlobReference(blobName);

            await blob.UploadTextAsync(fileContents);

            return blob.Uri.ToString();
        }

        private void EnsureBlobClient()
        {
            if (_container == null)
                Initialize();
        }

        public async Task<bool> BlobExistsAsync(string blobName)
        {
            EnsureBlobClient();
            var blob = await _container.Value.GetBlobReferenceFromServerAsync(blobName);

            return await blob.ExistsAsync(null, null);
        }

        public async Task<Stream> GetAsync(string blobName)
        {
            EnsureBlobClient();

            ICloudBlob blob = await _container.Value.GetBlobReferenceFromServerAsync(blobName);

            return await blob.OpenReadAsync(null, null, null);
        }
    }
}
