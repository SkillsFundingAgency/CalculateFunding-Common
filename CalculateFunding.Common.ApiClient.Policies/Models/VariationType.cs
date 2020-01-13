using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig
{
    public class VariationType
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("order")]
        public int Order { get; set; }
    }
}