using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Calcs.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Specifications.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Models;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Specifications
{
    [Obsolete("Move to the SpecificationsApiClient instead")]
    public class SpecsApiClient : BaseApiClient, ISpecsApiClient
    {
        private const string UrlRoot = "specs";

        public SpecsApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider)
           : base(httpClientFactory, HttpClientKeys.Specifications, logger, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<IEnumerable<Specification>>> GetSpecifications()
        {
            return await GetAsync<IEnumerable<Specification>>($"{UrlRoot}/specifications");
        }

        public async Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsSelectedForFunding()
        {
            return await GetAsync<IEnumerable<SpecificationSummary>>($"{UrlRoot}/specifications-selected-for-funding");
        }

        public async Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsSelectedForFundingByPeriod(string fundingPeriodId)
        {
            return await GetAsync<IEnumerable<SpecificationSummary>>($"{UrlRoot}/specifications-selected-for-funding-by-period?fundingPeriodId={fundingPeriodId}");
        }

        public async Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationSummaries()
        {
            return await GetAsync<IEnumerable<SpecificationSummary>>($"{UrlRoot}/specification-summaries");
        }

        public async Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecifications(string fundingPeriodId)
        {
            return await GetAsync<IEnumerable<SpecificationSummary>>($"{UrlRoot}/specifications-by-year?fundingPeriodId={fundingPeriodId}");
        }

        public async Task<ApiResponse<Specification>> GetSpecificationByName(string specificationName)
        {
            Guard.IsNullOrWhiteSpace(specificationName, nameof(specificationName));

            return await GetAsync<Specification>($"{UrlRoot}/specification-by-name?specificationName={specificationName}");
        }

        public async Task<ApiResponse<Specification>> GetSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<Specification>($"{UrlRoot}/specification-current-version-by-id?specificationId={specificationId}");
        }

        public async Task<ApiResponse<SpecificationSummary>> GetSpecificationSummary(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<SpecificationSummary>($"{UrlRoot}/specification-summary-by-id?specificationId={specificationId}");
        }

        public async Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationSummaries(IEnumerable<string> specificationIds)
        {
            Guard.ArgumentNotNull(specificationIds, nameof(specificationIds));

            if (!specificationIds.Any())
            {
                return new ApiResponse<IEnumerable<SpecificationSummary>>(HttpStatusCode.OK, Enumerable.Empty<SpecificationSummary>());
            }

            return await PostAsync<IEnumerable<SpecificationSummary>, IEnumerable<string>>($"{UrlRoot}/specification-summaries-by-ids", specificationIds);
        }

        public async Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetApprovedSpecifications(string fundingPeriodId, string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<IEnumerable<SpecificationSummary>>($"{UrlRoot}/specifications-by-fundingperiod-and-fundingstream?fundingPeriodId={fundingPeriodId}&fundingStreamId={fundingStreamId}");
        }

        public async Task<ValidatedApiResponse<Specification>> CreateSpecification(CreateSpecificationModel specification)
        {
            Guard.ArgumentNotNull(specification, nameof(specification));

            return await ValidatedPostAsync<Specification, CreateSpecificationModel>($"{UrlRoot}/specifications", specification);
        }

        public async Task<HttpStatusCode> UpdateSpecification(string specificationId, EditSpecificationModel specification)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.ArgumentNotNull(specification, nameof(specification));

            return await PutAsync($"{UrlRoot}/specification-edit?specificationId={specificationId}", specification);
        }

        public async Task<ValidatedApiResponse<Calculation>> CreateCalculation(CalculationCreateModel calculation)
        {
            Guard.ArgumentNotNull(calculation, nameof(calculation));

            return await ValidatedPostAsync<Calculation, CalculationCreateModel>($"{UrlRoot}/calculations", calculation);
        }

        public async Task<ValidatedApiResponse<Calculation>> UpdateCalculation(string specificationId, string calculationId, CalculationUpdateModel calculation)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));
            Guard.ArgumentNotNull(calculation, nameof(calculation));

            return await ValidatedPutAsync<Calculation, CalculationUpdateModel>($"{UrlRoot}/calculations?specificationId={specificationId}&calculationId={calculationId}", calculation);
        }

        [Obsolete]
        public async Task<ApiResponse<IEnumerable<Reference>>> GetFundingPeriods()
        {
            return await GetAsync<IEnumerable<Reference>>($"{UrlRoot}/get-fundingperiods");
        }

        [Obsolete]
        public async Task<ApiResponse<IEnumerable<FundingStream>>> GetFundingStreams()
        {
            return await GetAsync<IEnumerable<FundingStream>>($"{UrlRoot}/get-fundingstreams");
        }

        public async Task<ApiResponse<IEnumerable<FundingStream>>> GetFundingStreamsForSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<FundingStream>>($"{UrlRoot}/get-fundingstreams-for-specification?specificationId={specificationId}");
        }

        public async Task<ApiResponse<FundingStream>> GetFundingStreamByFundingStreamId(string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            return await GetAsync<FundingStream>($"{UrlRoot}/get-fundingstream-by-id?fundingstreamId={fundingStreamId}");
        }

        public async Task<ApiResponse<Calculation>> GetCalculationBySpecificationIdAndCalculationName(string specificationId, string calculationName)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(calculationName, nameof(calculationName));

            CalculationByNameRequestModel model = new CalculationByNameRequestModel { SpecificationId = specificationId, Name = calculationName };

            return await PostAsync<Calculation, CalculationByNameRequestModel>($"{UrlRoot}/calculation-by-name", model);
        }

        public async Task<ApiResponse<CalculationCurrentVersion>> GetCalculationById(string specificationId, string calculationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));

            return await GetAsync<CalculationCurrentVersion>($"{UrlRoot}/calculation-by-id?calculationId={calculationId}&specificationId={specificationId}");
        }

        public async Task<ApiResponse<IEnumerable<CalculationCurrentVersion>>> GetBaselineCalculationsBySpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<CalculationCurrentVersion>>($"{UrlRoot}/specifications/{specificationId}/baseline-calculations");
        }

        public async Task<PagedResult<SpecificationDatasourceRelationshipSearchResultItem>> FindSpecificationAndRelationships(SearchFilterRequest filterOptions)
        {
            Guard.ArgumentNotNull(filterOptions, nameof(filterOptions));

            SearchQueryRequest request = SearchQueryRequest.FromSearchFilterRequest(filterOptions);

            ApiResponse<SearchResults<SpecificationDatasourceRelationshipSearchResultItem>> results = await PostAsync<SearchResults<SpecificationDatasourceRelationshipSearchResultItem>, SearchQueryRequest>($"{UrlRoot}/specifications-dataset-relationships-search", request);
            if (results.StatusCode != HttpStatusCode.OK) return null;

            PagedResult<SpecificationDatasourceRelationshipSearchResultItem> result = new SearchPagedResult<SpecificationDatasourceRelationshipSearchResultItem>(filterOptions, results.Content.TotalCount)
            {
                Items = results.Content.Results
            };

            return result;
        }

        public async Task<PagedResult<SpecificationSearchResultItem>> FindSpecifications(SearchFilterRequest filterOptions)
        {
            SearchQueryRequest request = SearchQueryRequest.FromSearchFilterRequest(filterOptions);

            ApiResponse<SearchResults<SpecificationSearchResultItem>> results = await PostAsync<SearchResults<SpecificationSearchResultItem>, SearchQueryRequest>($"{UrlRoot}/specifications-search", request);
            if (results.StatusCode != HttpStatusCode.OK) return null;

            PagedResult<SpecificationSearchResultItem> result = new SearchPagedResult<SpecificationSearchResultItem>(filterOptions, results.Content.TotalCount)
            {
                Items = results.Content.Results,
                Facets = results.Content.Facets,
            };

            return result;
        }

        public async Task<ValidatedApiResponse<PublishStatusResult>> UpdatePublishStatus(string specificationId, PublishStatusEditModel model)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.ArgumentNotNull(model, nameof(model));

            return await ValidatedPutAsync<PublishStatusResult, PublishStatusEditModel>($"{UrlRoot}/specification-edit-status?specificationId={specificationId}", model);
        }

        public async Task<HttpStatusCode> SelectSpecificationForFunding(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PostAsync($"{UrlRoot}/select-for-funding?specificationId={specificationId}");
        }

        public async Task<ApiResponse<SpecificationCalculationExecutionStatusModel>> RefreshPublishedResults(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PostAsync<SpecificationCalculationExecutionStatusModel, string>($"{UrlRoot}/refresh-published-results?specificationId={specificationId}", specificationId);
        }

        public async Task<ApiResponse<SpecificationCalculationExecutionStatusModel>> CheckPublishResultStatus(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PostAsync<SpecificationCalculationExecutionStatusModel, string>($"{UrlRoot}/check-publish-result-status?specificationId={specificationId}", specificationId);
        }
    }
}