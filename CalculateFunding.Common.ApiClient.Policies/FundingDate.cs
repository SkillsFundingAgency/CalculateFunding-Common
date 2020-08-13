using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Policies
{
    public class FundingDate
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }

        [JsonProperty("fundingLineId")]
        public string FundingLineId { get; set; }

        [JsonProperty("patterns")]
        public IEnumerable<FundingDatePattern> Patterns { get; set; }
    }
}
