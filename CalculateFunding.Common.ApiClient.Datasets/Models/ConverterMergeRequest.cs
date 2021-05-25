using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class ConverterMergeRequest
    {
        public string ProviderVersionId { get; set; }

        public string DatasetId { get; set; }

        public string Version { get; set; }

        public Reference Author { get; set; }

        public string DatasetRelationshipId { get; set; }
    }
}