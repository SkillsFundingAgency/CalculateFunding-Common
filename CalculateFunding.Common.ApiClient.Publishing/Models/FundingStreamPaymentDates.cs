using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class FundingStreamPaymentDates
    {
        [JsonProperty("id")] 
        public string Id => $"{FundingStreamId}-{FundingPeriodId}";

        [JsonProperty("fundingStreamId")] 
        public string FundingStreamId { get; set; }

        [JsonProperty("fundingPeriodId")] 
        public string FundingPeriodId { get; set; }

        [JsonProperty("paymentDates")] 
        public IEnumerable<FundingStreamPaymentDate> PaymentDates { get; set; }
    }
}