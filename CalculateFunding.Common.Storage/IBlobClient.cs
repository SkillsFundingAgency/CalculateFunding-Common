using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;

namespace CalculateFunding.Common.Storage
{
    public interface IBlobClient
    {
        void Initialize();

        void VerifyFileName(string fileName);

        Task<(bool Ok, string Message)> IsHealthOk();

        Task<bool> DoesBlobExistAsync(string blobName);

        string GetBlobSasUrl(string blobName,
            DateTimeOffset finish,
            SharedAccessBlobPermissions permissions);

        Task<T> DownloadAsync<T>(string blobName);

        Task<string> UploadFileAsync<T>(string blobName, T contents);

        ICloudBlob GetBlockBlobReference(string blobName);

        Task<ICloudBlob> GetBlobReferenceFromServerAsync(string blobName);

        Task<Stream> DownloadToStreamAsync(ICloudBlob blob);

        Task<string> UploadFileAsync(string blobName, string fileContents);

        Task<bool> BlobExistsAsync(string blobName);

        Task<Stream> GetAsync(string blobName);
    }
}