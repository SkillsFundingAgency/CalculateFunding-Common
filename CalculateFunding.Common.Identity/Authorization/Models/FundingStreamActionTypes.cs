namespace CalculateFunding.Common.Identity.Authorization.Models
{
    public enum FundingStreamActionTypes
    {
        CanCreateSpecification,
        CanChooseFunding,
        CanCreateTemplates,
        CanEditTemplates,
        CanApproveTemplates,
        CanCreateProfilePattern,
        CanEditProfilePattern,
        CanAssignProfilePattern,
        CanApplyCustomProfilePattern,
        CanApproveCalculations,
        CanApproveAnyCalculations,
        CanRefreshPublishedQa,
        CanUploadDataSourceFiles,
    }
}
