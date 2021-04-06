using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Graph.Models
{
    [Serializable]
    public class FundingLine : SpecificationNode
    {
        [JsonProperty("fundinglineid")]
        public string FundingLineId { get; set; }

        [JsonProperty("fundinglinename")]
        public string FundingLineName { get; set; }
    }
}
