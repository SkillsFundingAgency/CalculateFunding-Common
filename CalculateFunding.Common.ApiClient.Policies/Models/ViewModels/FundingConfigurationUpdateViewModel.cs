using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Policies.Models.ViewModels
{
    public class FundingConfigurationUpdateViewModel
    {
        public IEnumerable<OrganisationGroupingConfiguration> OrganisationGroupings { get; set; }

        public string DefaultTemplateVersion { get; set; }
        
        public ApprovalMode ApprovalMode { get; set; }

        public IEnumerable<string> ErrorDetectors { get; set; }

        public UpdateCoreProviderVersion UpdateCoreProviderVersion { get; set; }

        public bool EnableUserEditableCustomProfiles { get; set; }

        public bool EnableUserEditableRuleBasedProfiles { get; set; }
        public bool RunCalculationEngineAfterCoreProviderUpdate { get; set; }

        public bool EnableConverterDataMerge { get; set; }

        public bool SuccessorCheck { get; set; }

        public IEnumerable<string> IndicativeOpenerProviderStatus { get; set; }

        public IEnumerable<string> AllowedPublishedFundingStreamsIdsToReference { get; set; }

        public IEnumerable<FundingVariation> ReleaseManagementVariations { get; set; }

        public IEnumerable<FundingConfigurationChannel> ReleaseChannels { get; set; }

    }
}
