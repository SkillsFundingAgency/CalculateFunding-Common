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
