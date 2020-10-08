using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Specifications.Models;
using CalculateFunding.Common.Models;
using CalculateFunding.Common.Models.Versioning;

namespace CalculateFunding.Common.ApiClient.Specifications
{
    public interface ISpecificationsApiClient
    {
        Task<ApiResponse<SpecificationSummary>> GetSpecificationSummaryById(string specificationId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsSelectedForFundingByPeriod(string fundingPeriodId);

        Task<HttpStatusCode> SetAssignedTemplateVersion(string specificationId, string templateVersion, string fundingStreamId);

        Task<HttpStatusCode> SelectSpecificationForFunding(string specificationId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsByFundingPeriodIdAndFundingStreamId(string fundingPeriodId, string fundingStreamId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationResultsByFundingPeriodIdAndFundingStreamId(string fundingPeriodId, string fundingStreamId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetApprovedSpecificationsByFundingPeriodIdAndFundingStreamId(string fundingPeriodId, string fundingStreamId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSelectedSpecificationsByFundingPeriodIdAndFundingStreamId(string fundingPeriodId, string fundingStreamId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsSelectedForFunding();

        Task<PagedResult<SpecificationSearchResultItem>> FindSpecifications(SearchFilterRequest filterOptions);

        Task<HttpStatusCode> SetPublishDates(string specificationId, SpecificationPublishDateModel specificationPublishDateModel);

        Task<ApiResponse<SpecificationPublishDateModel>> GetPublishDates(string specificationId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationSummaries();

        Task<ValidatedApiResponse<SpecificationSummary>> UpdateSpecification(string specificationId, EditSpecificationModel specification);

        /// <summary>
        /// Get Specification By Name
        /// </summary>
        /// <param name="specificationName">Specification Name</param>
        /// <returns>Specification when exists, null when it doesn't</returns>
        Task<ApiResponse<SpecificationSummary>> GetSpecificationByName(string specificationName);

        Task<ValidatedApiResponse<SpecificationSummary>> CreateSpecification(CreateSpecificationModel specification);

        Task<PagedResult<SpecificationDatasourceRelationshipSearchResultItem>> FindSpecificationAndRelationships(SearchFilterRequest filterOptions);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecifications(string fundingPeriodId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationSummaries(IEnumerable<string> specificationIds);

        Task<ApiResponse<IEnumerable<string>>> GetFundingStreamIdsForSelectedFundingSpecification();

        Task<ApiResponse<IEnumerable<Reference>>> GetFundingPeriodsByFundingStreamIds(string fundingStreamId);

        Task<ApiResponse<PublishStatusResponseModel>> UpdateSpecificationStatus(string specificationId, PublishStatusRequestModel publishStatusRequestModel);
        Task<HttpStatusCode> DeselectSpecificationForFunding(string specificationId);

        Task<ApiResponse<IEnumerable<string>>> GetDistinctFundingStreamsForSpecifications();

        Task<ApiResponse<bool>> DeleteSpecificationById(string specificationName);

        Task<ApiResponse<bool>> PermanentDeleteSpecificationById(string specificationName);

        Task<ApiResponse<IEnumerable<ProfileVariationPointer>>> GetProfileVariationPointers(string specificationId);

        Task<HttpStatusCode> SetProfileVariationPointer(string specificationId, ProfileVariationPointer profileVariationPointer);

        Task<HttpStatusCode> SetProfileVariationPointers(string specificationId, IEnumerable<ProfileVariationPointer> profileVariationPointer);

        Task<ApiResponse<IEnumerable<SpecificationReport>>> GetReportMetadataForSpecifications(string specificationId, string targetFundingPeriodId = null);
        
        Task<ApiResponse<SpecificationsDownloadModel>> DownloadSpecificationReport(string specificationReportIdentifier);
        Task<ApiResponse<JobModel>> ReIndexSpecification(string specificationId);

        Task<HttpStatusCode> SetProviderVersion(string specificationId, string providerVersionId);
    }
}