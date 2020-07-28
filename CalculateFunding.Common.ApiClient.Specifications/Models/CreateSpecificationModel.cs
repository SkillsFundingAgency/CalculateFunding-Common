using System.Collections.Generic;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    public class CreateSpecificationModel
    {
        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }

        [JsonProperty("fundingStreamIds")]
        public IEnumerable<string> FundingStreamIds { get; set; }

        [JsonProperty("providerVersionId")]
        public string ProviderVersionId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("assignedTemplateIds")]
        public IDictionary<string, string> AssignedTemplateIds { get; set; }
    }
}
