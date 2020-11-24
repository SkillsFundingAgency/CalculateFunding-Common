namespace CalculateFunding.Common.Identity.Authorization.Models
{
    public enum FundingStreamActionTypes
    {
        CanCreateSpecification,
        CanChooseFunding,
        CanCreateTemplates,
        CanEditTemplates,
        CanDeleteTemplates,
        CanApproveTemplates,
        CanCreateProfilePattern,
        CanEditProfilePattern,
        CanDeleteProfilePattern,
        CanAssignProfilePattern,
        CanApplyCustomProfilePattern,
        CanApproveCalculations,
        CanApproveAnyCalculations,
        CanRefreshPublishedQa,
        CanUploadDataSourceFiles,
    }
}
