using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    public class FundingVariation
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("order")]
        public int Order { get; set; }
        
        [JsonProperty("fundingLineCodes")]
        public IEnumerable<string> FundingLineCodes { get; set; }
    }
}