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
        CanReleaseFundingForStatement,
        CanReleaseFundingForPaymentOrContract,
        CanRefreshFunding,
        CanApproveSpecification,
        CanAdministerFundingStream,
        CanApproveCalculations,
        CanApproveAnyCalculations,
        CanApproveAllCalculations,
        CanRefreshPublishedQa,
    }
}
