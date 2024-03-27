using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.FDS.Models
{
    public class RemovedFieldDefinition
    {
        [JsonProperty("isActive")]
        public bool IsActive { get; set; }
        [JsonProperty("fundingDataDefinitions")]
        public List<FundingDataDefinition> FundingDataDefinitions { get; set; }
        [JsonProperty("inActiveDefinitions")]
        public List<InActiveDefinition> InActiveDefinitions { get; set; }
    }
}
