using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class ReProfileResponse
    {
        [JsonProperty("deliveryProfilePeriods")]
        public DeliveryProfilePeriod[] DeliveryProfilePeriods { get; set; }

        [JsonProperty("distributionPeriods")]
        public DistributionPeriods[] DistributionPeriods { get; set; }

        [JsonProperty("profilePatternKey")]
        public string ProfilePatternKey { get; set; }

        [JsonProperty("profilePatternDisplayName")]
        public string ProfilePatternDisplayName { get; set; }

        [JsonProperty("carryOverAmount")]
        public decimal CarryOverAmount { get; set; }

        [JsonProperty("skipReProfiling")]
        public bool SkipReProfiling { get; set; }

        [JsonProperty("strategyKey")]
        public string StrategyKey { get; set; }
    }
}