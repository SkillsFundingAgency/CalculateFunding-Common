using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class ProfilingPeriod
    {
        [JsonProperty("typeValue")]
        public string Period { get; set; }

        [JsonProperty("occurrence")]
        public int Occurrence { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("profileValue")]
        public decimal Value { get; set; }

        [JsonProperty("distributionPeriod")]
        public string DistributionPeriod { get; set; }

        [JsonProperty("calculationId")]
        public int? CalculationId { get; set; }
    }
}
