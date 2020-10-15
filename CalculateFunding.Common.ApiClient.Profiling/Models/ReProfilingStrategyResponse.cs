using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class ReProfilingStrategyResponse
    {
        [JsonProperty("strategyKey")]
        public string StrategyKey { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}