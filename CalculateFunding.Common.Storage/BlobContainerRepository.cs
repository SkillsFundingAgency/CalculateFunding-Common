using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace CalculateFunding.Common.Storage
{
    public class BlobContainerRepository : IBlobContainerRepository
    {
        public CloudBlobContainer GetCloudBlobContainer(BlobStorageOptions azureStorageSettings, string containerName = null)
        {
            CloudStorageAccount credentials = CloudStorageAccount.Parse(azureStorageSettings.ConnectionString);
            CloudBlobClient client = credentials.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerName?.ToLower() ?? azureStorageSettings.ContainerName.ToLower());

            container.CreateIfNotExists();

            return container;
        }
    }
}
