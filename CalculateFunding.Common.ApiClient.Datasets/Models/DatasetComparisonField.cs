using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetComparisonField
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }
}
