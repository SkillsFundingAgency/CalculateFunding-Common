using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig
{
    public class FundingConfiguration
    {
        [JsonProperty("organisationGroupings")]
        public IEnumerable<OrganisationGroupingConfiguration> OrganisationGroupings { get; set; }
            = Enumerable.Empty<OrganisationGroupingConfiguration>();

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("fundingStreamId")]
        public string FundingStreamId { get; set; }

        [JsonProperty("fundingPeriodId")]
        public string FundingPeriodId { get; set; }

        [JsonProperty("defaultTemplateVersion")]
        public string DefaultTemplateVersion { get; set; }

        [JsonProperty("variations")]
        public IEnumerable<FundingVariation> Variations { get; set; }
    }
}