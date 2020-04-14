using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ProfilePatternKey
    {
        [JsonProperty("fundingLineCode")]
        public string FundingLineCode { get; set; }
        
        [JsonProperty("key")]
        public string Key { get; set; }
    }
}