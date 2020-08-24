using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ProfilingCarryOver
    {
        [JsonProperty("fundingLineCode")]
        public string FundingLineCode { get; set; }
        
        [JsonProperty("type")]
        public ProfilingCarryOverType Type { get; set; }
        
        [JsonProperty("amount")]
        public decimal Amount { get; set; }
    }
}