using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Specifications.Models;

namespace CalculateFunding.Common.ApiClient.Specifications
{
    public interface ISpecificationsApiClient
    {
        Task<ApiResponse<SpecificationSummary>> GetSpecificationSummaryById(string specificationId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsSelectedForFundingByPeriod(string fundingPeriodId);

        Task<HttpStatusCode> SetAssignedTemplateVersion(string specificationId, string templateVersion, string fundingStreamId);

        Task<HttpStatusCode> SelectSpecificationForFunding(string specificationId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetApprovedSpecifications(string fundingPeriodId, string fundingStreamId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsSelectedForFunding();

        Task<PagedResult<SpecificationSearchResultItem>> FindSpecifications(SearchFilterRequest filterOptions);

        Task<ApiResponse<SpecificationPublishDateModel>> SetPublishDates(string specificationId, SpecificationPublishDateModel specificationPublishDateModel);

        Task<ApiResponse<SpecificationPublishDateModel>> GetPublishDates(string specificationId);
    }
}