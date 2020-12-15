using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Jobs.Models
{
    public class Outcome
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public OutcomeType Type { get; set; }

        [JsonProperty("jobDefinitionId")]
        public string JobDefinitionId { get; set; }

        [JsonProperty("isSuccessful")]
        public bool IsSuccessful { get; set; }
    }
}
