using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Calcs.Models;
using CalculateFunding.Common.ApiClient.Calcs.Models.Code;
using CalculateFunding.Common.ApiClient.Calcs.Models.ObsoleteItems;
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

        Task<ValidatedApiResponse<Calculation>> CreateCalculation(
            string specificationId,
            CalculationCreateModel calculationCreateModel,
            bool skipCalcRun,
            bool skipQueueCodeContextCacheUpdate,
            bool overrideCreateModelAuthor,
            bool updateBuildProject);

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

        Task<ApiResponse<IEnumerable<CalculationFundingLine>>> GetRootFundingLinesForCalculation(string calculationId);

        Task<ApiResponse<Job>> QueueCodeContextUpdate(string specificationId);

        Task<ApiResponse<Job>> QueueApproveAllSpecificationCalculations(string specificationId);


        Task<ApiResponse<IEnumerable<ObsoleteItem>>> GetObsoleteItemsForSpecification(string specificationId);
        Task<ApiResponse<IEnumerable<ObsoleteItem>>> GetObsoleteItemsForCalculation(string calculationId);
        Task<ApiResponse<ObsoleteItem>> CreateObsoleteItem(ObsoleteItem obsoleteItem);
        Task<HttpStatusCode> RemoveObsoleteItem(string obsoleteItemId, string calculationId);
        Task<HttpStatusCode> AddCalculationToObsoleteItem(string obsoleteItemId, string calculationId);
        Task<ValidatedApiResponse<Calculation>> EditCalculationWithSkipInstruct(string specificationId, string calculationId, CalculationEditModel calculationEditModel);
        Task<ApiResponse<Job>> ReMapSpecificationReference(string specificationId, string datasetDefinitionRelationshipId);
        Task<ApiResponse<Job>> QueueCalculationRun(string specificationId, QueueCalculationRunModel model);
        Task<ApiResponse<CalculationIdentifier>> GenerateCalculationIdentifier(GenerateIdentifierModel model);
        Task<ApiResponse<BuildProject>> GetCompiledBuildProjectBySpecificationId(string specificationId);
    }
}
