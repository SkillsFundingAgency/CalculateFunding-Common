namespace CalculateFunding.Common.FeatureToggles
{
    public interface IFeatureToggle
    {
        bool IsProviderProfilingServiceDisabled();

        bool IsPublishButtonEnabled();

		bool IsPublishAndApprovePageFiltersEnabled();

        bool IsCalculationTimeoutEnabled();

        bool IsRoleBasedAccessEnabled();

        bool IsNotificationsEnabled();

        bool IsNewEditCalculationPageEnabled();

        bool IsNewManageDataSourcesPageEnabled();

        bool IsNewProviderCalculationResultsIndexEnabled();

	    bool IsProviderInformationViewInViewFundingPageEnabled();

        bool IsDynamicBuildProjectEnabled();

        bool IsSearchModeAllEnabled();

        bool IsUseFieldDefinitionIdsInSourceDatasetsEnabled();

        bool IsProcessDatasetDefinitionNameChangesEnabled();

        bool IsProcessDatasetDefinitionFieldChangesEnabled();

        bool IsExceptionMessagesEnabled();

        bool IsCosmosDynamicScalingEnabled();
    }
}
