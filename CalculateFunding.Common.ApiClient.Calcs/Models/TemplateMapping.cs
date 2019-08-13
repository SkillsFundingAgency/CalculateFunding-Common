using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class TemplateMapping
    {
        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }

        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("templateMappingItems")]
        public IEnumerable<TemplateMappingItem> TemplateMappingItems { get; set; }
    }
}
