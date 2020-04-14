using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class FundingLineProfileOverrides
    {
        [JsonProperty("fundingLineCode")]
        public string FundingLineCode { get; set; }
        
        [JsonProperty("distributionPeriods")]
        public IEnumerable<DistributionPeriod> DistributionPeriods { get; set; }
    }
}