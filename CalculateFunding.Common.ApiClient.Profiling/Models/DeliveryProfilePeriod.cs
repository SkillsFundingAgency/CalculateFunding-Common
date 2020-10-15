using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class DeliveryProfilePeriod
    {
        [JsonProperty("typeValue")]
        public string TypeValue { get; set; }

        [JsonProperty("occurrence")]
        public int Occurrence { get; set; }

        [JsonProperty("type")]
        public PeriodType Type { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("profileValue")]
        public decimal ProfileValue { get; set; }

        [JsonProperty("distributionPeriod")]
        public string DistributionPeriod { get; set; }
    }
}