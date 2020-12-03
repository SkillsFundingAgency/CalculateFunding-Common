using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Providers.Models
{
    public class CurrentProviderVersionMetadata
    {
        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("providerVersionId")]
        public string ProviderVersionId { get; set; }

        [JsonProperty("providerSnapshotId")]
        public int? ProviderSnapshotId { get; set; }
    }
}
