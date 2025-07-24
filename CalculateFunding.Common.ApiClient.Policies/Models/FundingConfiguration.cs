using System;
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

        [JsonProperty("specToSpecChannelCode")]
        public string SpecToSpecChannelCode { get; set; }

        [JsonProperty("variations")]
        public IEnumerable<FundingVariation> Variations { get; set; }

        [JsonProperty("enableProfilingBasedOnOpenDate")]
        public bool EnableProfilingBasedOnOpenDate { get; set; }

        [JsonProperty("errorDetectors")]
        public IEnumerable<string> ErrorDetectors { get; set; }

        [JsonProperty("nonRestrictedProviderMetaData")]
        public IEnumerable<string> NonRestrictedProviderMetaData { get; set; }

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

        [JsonProperty("enableInformationLineAggregation")]
        public bool EnableInformationLineAggregation { get; set; }

        [JsonProperty("successorCheck")]
        public bool SuccessorCheck { get; set; }

        /// <summary>
        /// This property is used on PSG so that we don't populate the predecessor on the 
        /// current version from the provider on creation so that the property can be
        /// populated during the ClosureWithSuccessor variation strategy
        /// </summary>
        [JsonProperty("disablePopulatePredecessorOnCreate")]
        public bool DisablePopulatePredecessorOnCreate { get; set; }

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

        [JsonProperty("releaseActionGroups")]
        public IEnumerable<ReleaseActionGroup> ReleaseActionGroups { get; set; }

        [JsonProperty("enableCarryForward")]
        public bool EnableCarryForward { get; set; }

        [JsonProperty("useFDSData")]
        public Boolean useFDSData { get; set; }

        [JsonProperty("reprofilingOnDemandEnabled")]
        public bool ReprofilingOnDemandEnabled { get; set; }

        /// <summary>
        /// Variations to run during Reprofiling OnDemand Service
        /// </summary>
        [JsonProperty("reprofilingOnDemandVariations")]
        public IEnumerable<FundingVariation> ReprofilingOnDemandVariations { get; set; }

        /// <summary>
        /// Adult stream related config to find out the non applicable schemas
        /// </summary>
        [JsonProperty("adultNonApplicableSchemas")]
        public IEnumerable<string> AdultNonApplicableSchemas { get; set; }

        /// <summary>
        /// DisplayFundingPeriod flag used to display multiple funding period which
        /// help to create previous and future datasets.
        /// </summary>
        [JsonProperty("displayFundingPeriod")]
        public bool DisplayFundingPeriod {  get; set; }
    }
}