using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Calcs.Models;
using CalculateFunding.Common.ApiClient.Calcs.Models.Code;
using CalculateFunding.Common.ApiClient.Models;

namespace CalculateFunding.Common.ApiClient.Calcs
{
    public interface ICalculationsApiClient
    {
        Task<ApiResponse<IEnumerable<CalculationSummary>>> GetCalculationSummariesForSpecification(string specificationId);

        Task<ApiResponse<BuildProject>> GetBuildProjectBySpecificationId(string specificationId);

        Task<ApiResponse<byte[]>> GetAssemblyBySpecificationId(string specificationId);

        Task<ApiResponse<BuildProject>> UpdateBuildProjectRelationships(string specificationId, DatasetRelationshipSummary datasetRelationshipSummary);

        Task<ApiResponse<IEnumerable<Calculation>>> GetCalculationsForSpecification(string specificationId);

        Task<ApiResponse<HttpStatusCode>> CompileAndSaveAssembly(string specificationId);

        Task<ApiResponse<Calculation>> GetCalculationById(string calculationId);

        Task<ApiResponse<bool>> IsCalculationNameValid(string specificationId, string calculationName, string existingCalculationId = null);

        Task<ValidatedApiResponse<Calculation>> CreateCalculation(string specificationId, CalculationCreateModel calculationCreateModel);

        Task<ValidatedApiResponse<Calculation>> EditCalculation(string specificationId, string calculationId, CalculationEditModel calculationEditModel);

        Task<ApiResponse<PreviewResponse>> PreviewCompile(PreviewRequest previewRequest);

        Task<ApiResponse<IEnumerable<CalculationVersion>>> GetAllVersionsByCalculationId(string calculationId);

        Task<ApiResponse<IEnumerable<CalculationVersion>>> GetMultipleVersionsByCalculationId(IEnumerable<int> versionIds, string calculationId);

        Task<ApiResponse<IEnumerable<TypeInformation>>> GetCodeContextForSpecification(string specificationId);

        Task<ValidatedApiResponse<PublishStatusResult>> UpdatePublishStatus(string calculationId, PublishStatusEditModel model);

        Task<ApiResponse<IEnumerable<CalculationStatusCounts>>> GetCalculationStatusCounts(SpecificationIdsRequestModel request);

        Task<ApiResponse<SearchResults<CalculationSearchResult>>> FindCalculations(SearchFilterRequest filterOptions);

        Task<ApiResponse<IEnumerable<CalculationMetadata>>> GetCalculationMetadataForSpecification(string specificationId);

        Task<ApiResponse<TemplateMapping>> GetTemplateMapping(string specificationId, string fundingStreamId);

        Task<ApiResponse<BooleanResponseModel>> CheckHasAllApprovedTemplateCalculationsForSpecificationId(string specificationId);

        Task<ApiResponse<TemplateMapping>> ProcessTemplateMappings(string specificationId, string templateVersion, string fundingStreamId);

        Task<ApiResponse<SearchResults<CalculationSearchResult>>> SearchCalculationsForSpecification(string specificationId,
            CalculationType calculationType,
            PublishStatus? status,
            string searchTerm = null,
            int? page = null);
    }
}
