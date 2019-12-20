using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;

namespace CalculateFunding.Common.Storage
{
    public interface IBlobClient
    {
        void VerifyFileName(string fileName);

        Task<(bool Ok, string Message)> IsHealthOk();

        Task<bool> DoesBlobExistAsync(string blobName, string containerName = null);

        string GetBlobSasUrl(string blobName,
            DateTimeOffset finish,
            SharedAccessBlobPermissions permissions, string containerName = null);

        Task<T> DownloadAsync<T>(string blobName, string containerName = null);

        Task<string> UploadFileAsync<T>(string blobName, T contents, string containerName = null);

        ICloudBlob GetBlockBlobReference(string blobName, string containerName = null);

        Task<ICloudBlob> GetBlobReferenceFromServerAsync(string blobName, string containerName = null);

        Task<Stream> DownloadToStreamAsync(ICloudBlob blob);

        Task<string> UploadFileAsync(string blobName, string fileContents, string containerName = null);

        Task<bool> BlobExistsAsync(string blobName, string containerName = null);

        Task<Stream> GetAsync(string blobName, string containerName = null);

        Task BatchProcessBlobs(Func<IEnumerable<IListBlobItem>, Task> batchProcessor, 
            string containerName = null, 
            int batchSize = 50);
    }
}