using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class PublishedProivderFundingStreamFunding
    {
        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("totalFunding")]
        public decimal? TotalFunding { get; set; }
    }
}