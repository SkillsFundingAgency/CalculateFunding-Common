using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class PublishedProviderFundingCount
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        
        [JsonProperty("totalFunding")]
        public decimal? TotalFunding { get; set; }     
    }
}