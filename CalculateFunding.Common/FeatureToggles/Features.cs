using Microsoft.Extensions.Configuration;

namespace CalculateFunding.Common.FeatureToggles
{
    public class Features : IFeatureToggle
    {
        private readonly IConfigurationSection _config;

        public Features(IConfigurationSection config)
        {
            _config = config;
        }

        public bool IsAllocationLineMajorMinorVersioningEnabled()
        {
            return CheckSetting("allocationLineMajorMinorVersioningEnabled");
        }

        public bool IsProviderProfilingServiceDisabled()
        {
            return CheckSetting("providerProfilingServiceDisabled");
        }

        public bool IsAggregateSupportInCalculationsEnabled()
        {
            return CheckSetting("aggregateSupportInCalculationsEnabled");
        }

        public bool IsPublishButtonEnabled()
        {
            return CheckSetting("publishButtonEnabled");
        }

        public bool IsCalculationTimeoutEnabled()
        {
            return CheckSetting("calculationTimeoutEnabled");
        }

		public bool IsPublishAndApprovePageFiltersEnabled()
		{
			return CheckSetting("publishAndApprovePageFiltersEnabled");
		}

        public bool IsRoleBasedAccessEnabled()
        {
            return CheckSetting("roleBasedAccessEnabled");
        }

        public bool IsNotificationsEnabled()
        {
            return CheckSetting("notificationsEnabled");
        }

        public bool IsAggregateOverCalculationsEnabled()
        {
            return CheckSetting("aggregateOverCalculationsEnabled");
        }

        public bool IsNewEditCalculationPageEnabled()
        {
            return CheckSetting("newEditCalculationPageEnabled");
        }

        public bool IsNewManageDataSourcesPageEnabled()
        {
            return CheckSetting("newManageDataSourcesPageEnabled");
        }

        public bool IsAllAllocationResultsVersionsInFeedIndexEnabled()
        {
            return CheckSetting("allAllocationResultsVersionsInFeedIndexEnabled");
        }

        public bool IsProviderVariationsEnabled()
        {
            return CheckSetting("providerVariationsEnabled");
        }

        public bool IsNewProviderCalculationResultsIndexEnabled()
        {
            return CheckSetting("newProviderCalculationResultsIndexEnabled");
        }

	    public bool IsProviderInformationViewInViewFundingPageEnabled()
	    {
		    return CheckSetting("providerInformationViewInViewFundingPageEnabled");
	    }

        public bool IsCheckJobStatusForChooseAndRefreshEnabled()
        {
            return CheckSetting("checkJobStatusForChooseAndRefreshEnabled");
        }

        public bool IsDuplicateCalculationNameCheckEnabled()
        {
            return CheckSetting("duplicateCalculationNameCheckEnabled");
        }

        public bool IsDynamicBuildProjectEnabled()
        {
            return CheckSetting("dynamicBuildProjectEnabled");
        }

        public bool IsSearchModeAllEnabled()
        {
            return CheckSetting("searchModeAllEnabled");
        }

        public bool IsUseFieldDefinitionIdsInSourceDatasetsEnabled()
        {
            return CheckSetting("useFieldDefinitionIdsInSourceDatasetsEnabled");
        }

        private bool CheckSetting(string featureName)
        {
            if (_config == null)
            {
                return false;
            }

            string value = _config[featureName];

            if (string.IsNullOrEmpty(value))
            {
                return false;
            }
            else
            {
                if (bool.TryParse(value, out var result))
                {
                    return result;
                }
                else
                {
                    return false;
                }
            }
        }
	}
}
