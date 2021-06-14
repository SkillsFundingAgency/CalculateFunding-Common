using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetVersionViewModel
    {
        public Reference Author { get; set; }

        public int Version { get; set; }

        public string BlobName { get; set; }

        public string ProviderVersionId { get; set; }

        public DatasetChangeType ChangeType { get; set; }

        public Reference FundingStream { get; set; }
    }
}