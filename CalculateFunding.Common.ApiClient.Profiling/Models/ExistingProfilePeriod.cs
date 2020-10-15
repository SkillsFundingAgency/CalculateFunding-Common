using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class ExistingProfilePeriod
    {
        [JsonProperty("typeValue")]
        public string TypeValue { get; set; }

        [JsonProperty("occurrence")]
        public int Occurrence { get; set; }

        [JsonProperty("type")]
        public PeriodType Type { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        /// <summary>
        /// Profile value. If this value is null, then the re-profiling should generate the value, otherwise the value is considered by either paid or to be set to this value
        /// </summary>
        [JsonProperty("profileValue")]
        public decimal? ProfileValue { get; set; }

        [JsonProperty("distributionPeriod")]
        public string DistributionPeriod { get; set; }
    }
}