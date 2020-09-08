using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Calcs.Models;
using CalculateFunding.Common.ApiClient.Calcs.Models.Code;
using  CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Calcs
{
    public class CalculationsApiClient : BaseApiClient, ICalculationsApiClient
    {
        private const string UrlRoot = "calcs";

        public CalculationsApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
         : base(httpClientFactory, HttpClientKeys.Calculations, logger, cancellationTokenProvider)
        { }
        
        public async Task<ApiResponse<SearchResults<CalculationSearchResult>>> SearchCalculationsForSpecification(string specificationId,
            CalculationType calculationType,
            PublishStatus? status,
            string searchTerm = null,
            int? page = null)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            
            string url = $"specifications/{specificationId}/calculations/calculationType/{calculationType}";
            bool hasQueryString = false;
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                hasQueryString = true;
                
                url = $"{url}?searchTerm={ WebUtility.UrlEncode(searchTerm)}";
            }

            if (page.HasValue)
            {
                url = hasQueryString ? $"{url}&page={page}" : $"{url}?page={page}";
                
                hasQueryString = true;
            }

            if (status.HasValue)
            {
                url = hasQueryString ? $"{url}&status={status}" : $"{url}?status={status}";
            }
            
            return await GetAsync<SearchResults<CalculationSearchResult>>(url);
        }	

        public async Task<ApiResponse<IEnumerable<CalculationSummary>>> GetCalculationSummariesForSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"{UrlRoot}/calculation-summaries-for-specification?specificationId={specificationId}";

            return await GetAsync<IEnumerable<CalculationSummary>>(url);
        }

        public async Task<ApiResponse<BuildProject>> GetBuildProjectBySpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"{UrlRoot}/get-buildproject-by-specification-id?specificationId={specificationId}";

            return await GetAsync<BuildProject>(url);
        }

        public async Task<ApiResponse<byte[]>> GetAssemblyBySpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"{UrlRoot}/{specificationId}/assembly";

            return await GetAsync<byte[]>(url);
        }

        public async Task<ApiResponse<BuildProject>> UpdateBuildProjectRelationships(string specificationId, DatasetRelationshipSummary datasetRelationshipSummary)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.ArgumentNotNull(datasetRelationshipSummary, nameof(datasetRelationshipSummary));

            string url = $"{UrlRoot}/update-buildproject-relationships?specificationId={specificationId}";

            return await PostAsync<BuildProject, DatasetRelationshipSummary>(url, datasetRelationshipSummary);
        }

        public async Task<ApiResponse<IEnumerable<Calculation>>> GetCalculationsForSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"{UrlRoot}/current-calculations-for-specification?specificationId={specificationId}";

            return await GetAsync<IEnumerable<Calculation>>(url);
        }

        public async Task<ApiResponse<HttpStatusCode>> CompileAndSaveAssembly(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"{UrlRoot}/{specificationId}/compileAndSaveAssembly";

            HttpStatusCode httpStatusCode = await GetAsync(url);

            return new ApiResponse<HttpStatusCode>(HttpStatusCode.OK, httpStatusCode);
        }

        public async Task<ApiResponse<Calculation>> GetCalculationById(string calculationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));

            string url = $"{UrlRoot}/calculation-by-id?calculationId={calculationId}";

            return await GetAsync<Calculation>(url);
        }

        public async Task<ApiResponse<bool>> IsCalculationNameValid(string specificationId, string calculationName, string existingCalculationId = null)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(calculationName, nameof(calculationName));

            string url = $"{UrlRoot}/validate-calc-name/{specificationId}/{calculationName}/{existingCalculationId}";

            HttpStatusCode httpStatusCode = await GetAsync(url);

            return new ApiResponse<bool>(HttpStatusCode.OK, httpStatusCode == HttpStatusCode.OK);
        }

        public async Task<ValidatedApiResponse<Calculation>> CreateCalculation(string specificationId, CalculationCreateModel calculationCreateModel)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.ArgumentNotNull(calculationCreateModel, nameof(calculationCreateModel));

            string url = $"{UrlRoot}/specifications/{specificationId}/calculations";

            return await ValidatedPostAsync<Calculation, CalculationCreateModel>(url, calculationCreateModel);
        }

        public async Task<ValidatedApiResponse<Calculation>> EditCalculation(string specificationId, string calculationId, CalculationEditModel calculationEditModel)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.ArgumentNotNull(calculationEditModel, nameof(calculationEditModel));

            string url = $"{UrlRoot}/specifications/{specificationId}/calculations/{calculationId}";

            return await ValidatedPutAsync<Calculation, CalculationEditModel>(url, calculationEditModel);
        }

        public async Task<ApiResponse<PreviewResponse>> PreviewCompile(PreviewRequest previewRequest)
        {
            Guard.ArgumentNotNull(previewRequest, nameof(previewRequest));

            string url = $"{UrlRoot}/compile-preview";

            return await PostAsync<PreviewResponse, PreviewRequest>(url, previewRequest);
        }

        public async Task<ApiResponse<IEnumerable<CalculationVersion>>> GetAllVersionsByCalculationId(string calculationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));

            string url = $"{UrlRoot}/calculation-version-history?calculationId={calculationId}";

            return await GetAsync<IEnumerable<CalculationVersion>>(url);
        }

        public async Task<ApiResponse<IEnumerable<CalculationVersion>>> GetMultipleVersionsByCalculationId(IEnumerable<int> versionIds, string calculationId)
        {
            Guard.ArgumentNotNull(versionIds, nameof(versionIds));
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));

            CalculationVersionsCompareModel calcsVersGetModel = new CalculationVersionsCompareModel()
            {
                Versions = versionIds,
                CalculationId = calculationId,
            };

            string url = $"{UrlRoot}/calculation-versions";

            return await PostAsync<IEnumerable<CalculationVersion>, CalculationVersionsCompareModel>(url, calcsVersGetModel);
        }

        public async Task<ApiResponse<IEnumerable<TypeInformation>>> GetCodeContextForSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"{UrlRoot}/get-calculation-code-context?specificationId={specificationId}";

            return await GetAsync<IEnumerable<TypeInformation>>(url);
        }

        public async Task<ValidatedApiResponse<PublishStatusResult>> UpdatePublishStatus(string calculationId, PublishStatusEditModel model)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.ArgumentNotNull(model, nameof(model));

            string url = $"{UrlRoot}/calculation-edit-status?calculationId={calculationId}";

            return await ValidatedPutAsync<PublishStatusResult, PublishStatusEditModel>(url, model);
        }

        public async Task<ApiResponse<IEnumerable<CalculationStatusCounts>>> GetCalculationStatusCounts(SpecificationIdsRequestModel request)
        {
            Guard.ArgumentNotNull(request, nameof(request));

            string url = $"{UrlRoot}/status-counts";

            return await PostAsync<IEnumerable<CalculationStatusCounts>, SpecificationIdsRequestModel>(url, request);
        }

        public async Task<ApiResponse<SearchResults<CalculationSearchResult>>> FindCalculations(SearchFilterRequest filterOptions)
        {
            Guard.ArgumentNotNull(filterOptions, nameof(filterOptions));

            SearchQueryRequest request = SearchQueryRequest.FromSearchFilterRequest(filterOptions);

            string url = $"{UrlRoot}/calculations-search";

            ApiResponse<SearchResults<CalculationSearchResult>> results = await PostAsync<SearchResults<CalculationSearchResult>, SearchQueryRequest>(url, request);

            return results;
        }

        public async Task<ApiResponse<IEnumerable<CalculationMetadata>>> GetCalculationMetadataForSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"{UrlRoot}/specifications/{specificationId}/calculations/metadata";

            return await GetAsync<IEnumerable<CalculationMetadata>>(url);
        }

        public async Task<ApiResponse<TemplateMapping>> GetTemplateMapping(string specificationId, string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            string url = $"{UrlRoot}/specifications/{specificationId}/templatemapping/{fundingStreamId}";

            return await GetAsync<TemplateMapping>(url);
        }

        public async Task<ApiResponse<BooleanResponseModel>> CheckHasAllApprovedTemplateCalculationsForSpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"{UrlRoot}/specifications/{specificationId}/templateCalculations/allApproved";

            return await GetAsync<BooleanResponseModel>(url);
        }

        public async Task<ApiResponse<TemplateMapping>> ProcessTemplateMappings(string specificationId, string templateVersion, string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            string url = $"{UrlRoot}/specifications/{specificationId}/templates/{fundingStreamId}";

            return await PutAsync<TemplateMapping, string>(url, templateVersion);
        }

        public async Task<ApiResponse<IEnumerable<CalculationFundingLine>>> GetRootFundingLinesForCalculation(string calculationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));

            return await GetAsync<IEnumerable<CalculationFundingLine>>($"{UrlRoot}/{calculationId}/root-funding-lines");
        }
    }
}
