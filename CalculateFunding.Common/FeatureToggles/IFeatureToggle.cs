namespace CalculateFunding.Common.FeatureToggles
{
    public interface IFeatureToggle
    {
        bool IsProviderProfilingServiceDisabled();

        bool IsAllocationLineMajorMinorVersioningEnabled();

        bool IsAggregateSupportInCalculationsEnabled();

        bool IsPublishButtonEnabled();

		bool IsPublishAndApprovePageFiltersEnabled();

        bool IsCalculationTimeoutEnabled();

        bool IsRoleBasedAccessEnabled();

        bool IsNotificationsEnabled();

        bool IsAggregateOverCalculationsEnabled();

        bool IsNewEditCalculationPageEnabled();

        bool IsNewManageDataSourcesPageEnabled();

        bool IsAllAllocationResultsVersionsInFeedIndexEnabled();

        bool IsProviderVariationsEnabled();

        bool IsNewProviderCalculationResultsIndexEnabled();

	    bool IsProviderInformationViewInViewFundingPageEnabled();

        bool IsCheckJobStatusForChooseAndRefreshEnabled();

        bool IsDuplicateCalculationNameCheckEnabled();

        bool IsDynamicBuildProjectEnabled();

        bool IsSearchModeAllEnabled();

        bool IsUseFieldDefinitionIdsInSourceDatasetsEnabled();
    }
}
