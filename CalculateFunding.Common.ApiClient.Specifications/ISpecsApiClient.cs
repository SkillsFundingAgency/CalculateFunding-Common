using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Calcs.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Specifications.Models;
using CalculateFunding.Common.Models;
using FundingStream = CalculateFunding.Common.ApiClient.Policies.Models.FundingStream;

namespace CalculateFunding.Common.ApiClient.Specifications
{
    public interface ISpecsApiClient
    {
        Task<ApiResponse<Specification>> GetSpecification(string specificationId);

        Task<ApiResponse<SpecificationSummary>> GetSpecificationSummary(string specificationId);

        /// <summary>
        /// Get Specification By Name
        /// </summary>
        /// <param name="specificationName">Specification Name</param>
        /// <returns>Specification when exists, null when it doesn't</returns>
        Task<ApiResponse<Specification>> GetSpecificationByName(string specificationName);

        /// <summary>
        /// Gets all Specifications
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<Specification>>> GetSpecifications();

        /// <summary>
        /// Get all specifications which have been selected for funding
        /// </summary>
        /// <returns>Specifications</returns>
        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsSelectedForFunding();

        /// <summary>
        /// Gets all Specification Summaries
        /// </summary>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationSummaries();

        /// <summary>
        /// Gets all Specification Summaries given provided Specification IDs
        /// </summary>
        /// <returns></returns>
        /// <param name="specificationIds">Specification IDs</param>
        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationSummaries(IEnumerable<string> specificationIds);

        /// <summary>
        /// Get approved or updated specifications for a given Funding Period and Funding Stream
        /// </summary>
        /// <param name="fundingPeriodId">Funding Period Id</param>
        /// <param name="fundingStreamId">Funding Stream Id</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetApprovedSpecifications(string fundingPeriodId, string fundingStreamId);

        /// <summary>
        /// Gets Specifications by Academic Year ID
        /// </summary>
        /// <param name="fundingPeriodId">Academic Year Id</param>
        /// <returns></returns>
        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecifications(string fundingPeriodId);

        Task<ValidatedApiResponse<Specification>> CreateSpecification(CreateSpecificationModel specification);

        Task<ApiResponse<IEnumerable<Reference>>> GetFundingPeriods();

        Task<ApiResponse<IEnumerable<FundingStream>>> GetFundingStreams();

        Task<ApiResponse<IEnumerable<FundingStream>>> GetFundingStreamsForSpecification(string specificationId);

        Task<ApiResponse<Calculation>> GetCalculationBySpecificationIdAndCalculationName(string specificationId, string calculationName);

        Task<ApiResponse<CalculationCurrentVersion>> GetCalculationById(string specificationId, string calculationId);

        Task<ApiResponse<IEnumerable<CalculationCurrentVersion>>> GetBaselineCalculationsBySpecificationId(string specificationId);

        Task<ValidatedApiResponse<Calculation>> CreateCalculation(CalculationCreateModel calculation);

        Task<ValidatedApiResponse<Calculation>> UpdateCalculation(string specificationId, string calculationId, CalculationUpdateModel calculation);

        Task<ApiResponse<FundingStream>> GetFundingStreamByFundingStreamId(string fundingStreamId);

        Task<PagedResult<SpecificationSearchResultItem>> FindSpecifications(SearchFilterRequest filterOptions);

        Task<PagedResult<SpecificationDatasourceRelationshipSearchResultItem>> FindSpecificationAndRelationships(SearchFilterRequest filterOptions);

        Task<HttpStatusCode> UpdateSpecification(string specificationId, EditSpecificationModel specification);

        Task<ValidatedApiResponse<PublishStatusResult>> UpdatePublishStatus(string specificationId, PublishStatusEditModel model);

        Task<HttpStatusCode> SelectSpecificationForFunding(string specificationId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsSelectedForFundingByPeriod(string fundingPeriodId);

        Task<ApiResponse<SpecificationCalculationExecutionStatusModel>> RefreshPublishedResults(string specificationId);

        Task<ApiResponse<SpecificationCalculationExecutionStatusModel>> CheckPublishResultStatus(string specificationId);
    }
}