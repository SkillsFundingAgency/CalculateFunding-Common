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

        bool IsJobServiceEnabled();

        bool IsNotificationsEnabled();

        bool IsAggregateOverCalculationsEnabled();

        bool IsCalculationResultsNotificationsEnabled();

        bool IsNewEditCalculationPageEnabled();

        bool IsNewManageDataSourcesPageEnabled();

        bool IsJobServiceForMainActionsEnabled();

        bool IsAllAllocationResultsVersionsInFeedIndexEnabled();

        bool IsApprovalBatchingServerSideEnabled();

        bool IsJobServiceForPublishProviderResultsEnabled();

        bool IsProviderVariationsEnabled();

        bool IsNewProviderCalculationResultsIndexEnabled();

	    bool IsProviderInformationViewInViewFundingPageEnabled();

        bool IsCheckJobStatusForChooseAndRefreshEnabled();
    }
}
