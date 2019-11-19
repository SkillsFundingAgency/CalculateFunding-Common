using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ProviderFundingStreamStatusResponse
    {
        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("providerDraftCount")]
        public int ProviderDraftCount { get; set; }
        
        [JsonProperty("providerApprovedCount")]
        public int ProviderApprovedCount { get; set; }
        
        [JsonProperty("providerUpdatedCount")]
        public int ProviderUpdatedCount { get; set; }
        
        [JsonProperty("providerReleasedCount")]
        public int ProviderReleasedCount { get; set; }
    }
}
