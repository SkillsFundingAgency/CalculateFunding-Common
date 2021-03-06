using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    public  class SpecificationInformation
    {
        [JsonProperty("id")]
        public string Id { get; set; }
            
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("lastEditDate")]
        public DateTimeOffset? LastEditDate { get; set; }
        
        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }
        
        [JsonProperty("fundingStreamIds")]
        public IEnumerable<string> FundingStreamIds { get; set; }
            
        [JsonProperty("fundingPeriodEnd")]
        public DateTimeOffset? FundingPeriodEnd { get; set; }
    }
}