using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    public class FundingStructureItem
    {
        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("fundingLineCode")]
        public string FundingLineCode { get; set; }

        [JsonProperty("calculationId")]
        public string CalculationId { get; set; }

        [JsonProperty("templateId")]
        public uint TemplateId { get; set; }

        [JsonProperty("type")]
        public FundingStructureType Type { get; set; }

        [JsonProperty("calculationType")]
        public string CalculationType { get; set; }

        [JsonProperty("fundingStructureItems")]
        public ICollection<FundingStructureItem> FundingStructureItems { get; set; }
    }
}
