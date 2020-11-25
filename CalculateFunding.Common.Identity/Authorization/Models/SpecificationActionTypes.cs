namespace CalculateFunding.Common.Identity.Authorization.Models
{
    public enum SpecificationActionTypes
    {
        CanEditSpecification,
        CanEditCalculations,
        CanCreateCalculations,
        CanMapDatasets,
        CanChooseFunding,
        CanApproveFunding,
        CanReleaseFunding,
        CanRefreshFunding,
        CanCreateQaTests,
        CanEditQaTests,
        CanApproveSpecification,
        CanAdministerFundingStream,
        CanDeleteSpecification,
        CanDeleteCalculations,
        CanDeleteQaTests,
        CanApproveCalculations,
        CanApproveAnyCalculations,
        CanApproveAllCalculations,
        CanRefreshPublishedQa,
    }
}
