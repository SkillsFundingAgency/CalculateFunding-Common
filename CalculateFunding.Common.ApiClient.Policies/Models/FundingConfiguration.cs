using System.Collections.Generic;
using CalculateFunding.Common.ApiClient.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Policies.Models.FundingConfig
{
    public class FundingConfiguration
    {
        [JsonProperty("organisationGroupings")]
        public IEnumerable<OrganisationGroupingConfiguration> OrganisationGroupings { get; set; }
            = new List<OrganisationGroupingConfiguration>();

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

        [JsonProperty("errorDetectors")]
        public IEnumerable<string> ErrorDetectors { get; set; }

        [JsonProperty("approvalMode")]
        public ApprovalMode ApprovalMode { get; set; }

        [JsonProperty("providerSource")]
        public ProviderSource ProviderSource { get; set; }

        [JsonProperty("paymentOrganisationSource")]
        public PaymentOrganisationSource PaymentOrganisationSource { get; set; }

        [JsonProperty("updateCoreProviderVersion")]
        public UpdateCoreProviderVersion UpdateCoreProviderVersion { get; set; }

        [JsonProperty("enableUserEditableCustomProfiles")]
        public bool EnableUserEditableCustomProfiles { get; set; }

        [JsonProperty("enableUserEditableRuleBasedProfiles")]
        public bool EnableUserEditableRuleBasedProfiles { get; set; }

        [JsonProperty("runCalculationEngineAfterCoreProviderUpdate")]
        public bool RunCalculationEngineAfterCoreProviderUpdate { get; set; }

        [JsonProperty("enableConverterDataMerge")]
        public bool EnableConverterDataMerge { get; set; }

        [JsonProperty("successorCheck")]
        public bool SuccessorCheck { get; set; }

        [JsonProperty("indicativeOpenerProviderStatus")]
        public IEnumerable<string> IndicativeOpenerProviderStatus { get; set; }

        [JsonProperty("allowedPublishedFundingStreamsIdsToReference")]
        public IEnumerable<string> AllowedPublishedFundingStreamsIdsToReference { get; set; }

        /// <summary>
        /// Variations to run during release management
        /// </summary>
        [JsonProperty("releaseManagementVariations")]
        public IEnumerable<FundingVariation> ReleaseManagementVariations { get; set; }

        [JsonProperty("releaseChannels")]
        public IEnumerable<FundingConfigurationChannel> ReleaseChannels { get; set; }
    }    
}