﻿using Microsoft.Extensions.Configuration;

namespace CalculateFunding.Common.FeatureToggles
{
    public class Features : IFeatureToggle
    {
        private readonly IConfigurationSection _config;

        public Features(IConfigurationSection config)
        {
            _config = config;
        }

        

        public bool IsProviderProfilingServiceDisabled()
        {
            return CheckSetting("providerProfilingServiceDisabled");
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

        public bool IsNewEditCalculationPageEnabled()
        {
            return CheckSetting("newEditCalculationPageEnabled");
        }

        public bool IsNewManageDataSourcesPageEnabled()
        {
            return CheckSetting("newManageDataSourcesPageEnabled");
        }

        public bool IsNewProviderCalculationResultsIndexEnabled()
        {
            return CheckSetting("newProviderCalculationResultsIndexEnabled");
        }

	    public bool IsProviderInformationViewInViewFundingPageEnabled()
	    {
		    return CheckSetting("providerInformationViewInViewFundingPageEnabled");
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

        public bool IsProcessDatasetDefinitionNameChangesEnabled()
        {
            return CheckSetting("processDatasetDefinitionNameChangesEnabled");
        }

        public bool IsProcessDatasetDefinitionFieldChangesEnabled()
        {
            return CheckSetting("processDatasetDefinitionFieldChangesEnabled");
        }

        public bool IsExceptionMessagesEnabled()
        {
            return CheckSetting("exceptionMessagesEnabled");
        }

        public bool IsCosmosDynamicScalingEnabled()
        {
            return CheckSetting("cosmosDynamicScalingEnabled");
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
