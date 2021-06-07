using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class PublishedProviderFundingCount
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("paidProviderCount")]
        public int PaidProviderCount { get; set; }

        [JsonProperty("indicativeProviderCount")]
        public int IndicativeProviderCount { get; set; }

        [JsonProperty("providerTypes")]
        public IEnumerable<ProviderTypeSubType> ProviderTypes { get; set; }

        [JsonProperty("providerTypesCount")]
        public int ProviderTypesCount { get; set; }

        [JsonProperty("localAuthorities")]
        public IEnumerable<string> LocalAuthorities { get; set; }

        [JsonProperty("localAuthoritiesCount")]
        public int LocalAuthoritiesCount { get; set; }

        [JsonProperty("fundingStreamsFundings")]
        public IEnumerable<PublishedProivderFundingStreamFunding> FundingStreamsFundings { get; set; }

        [JsonProperty("totalFunding")]
        public decimal? TotalFunding { get; set; }

        [JsonProperty("paidProvidersTotalFunding")]
        public decimal? PaidProvidersTotalFunding { get; set; }

        [JsonProperty("indicativeProviderTotalFunding")]
        public decimal? IndicativeProviderTotalFunding { get; set; }
    }
}