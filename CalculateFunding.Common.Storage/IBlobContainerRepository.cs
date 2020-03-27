using Microsoft.Azure.Storage.Blob;

namespace CalculateFunding.Common.Storage
{
    public interface IBlobContainerRepository
    {
        CloudBlobContainer GetCloudBlobContainer(BlobStorageOptions azureStorageSettings, string containerName = null);
    }
}
