using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace CalculateFunding.Common.Storage.UnitTests
{
    public class BlobContainerRepositoryTest : BlobContainerRepository
    {
        private CloudBlobContainer _cloudBlobContainer;

        public BlobContainerRepositoryTest(BlobStorageOptions blobStorageOptions, CloudBlobContainer cloudBlobContainer) : base(blobStorageOptions)
        {
            _cloudBlobContainer = cloudBlobContainer;
        }

        public override CloudBlobContainer GetOrCreateContainer(string containerName)
        {
            return _cloudBlobContainer;
        }
    }
}
