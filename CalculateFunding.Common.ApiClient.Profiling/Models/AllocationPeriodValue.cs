using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class AllocationPeriodValue
    {
        [JsonProperty("distributionPeriod")]
        public string DistributionPeriod { get; set; }

        [JsonProperty("allocationValue")]
        public decimal AllocationValue { get; set; }
    }
}
