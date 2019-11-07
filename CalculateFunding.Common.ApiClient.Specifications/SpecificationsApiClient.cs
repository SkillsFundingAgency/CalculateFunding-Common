using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Specifications.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Specifications
{
    public class SpecificationsApiClient : BaseApiClient, ISpecificationsApiClient
    {
        private const string UrlRoot = "specs";

        public SpecificationsApiClient(IHttpClientFactory httpClientFactory,
            ILogger logger,
            ICancellationTokenProvider cancellationTokenProvider = null)
            : base(httpClientFactory, HttpClientKeys.Specifications, logger, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<SpecificationSummary>> GetSpecificationSummaryById(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<SpecificationSummary>(
                $"{UrlRoot}/specification-summary-by-id?specificationId={specificationId}");
        }

        public async Task<ApiResponse<SpecificationSummary>> GetSpecificationByName(string specificationName)
        {
            Guard.IsNullOrWhiteSpace(specificationName, nameof(specificationName));

            return await GetAsync<SpecificationSummary>($"{UrlRoot}/specification-by-name?specificationName={specificationName}");
        }

        public async Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsSelectedForFundingByPeriod(
            string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            return await GetAsync<IEnumerable<SpecificationSummary>>(
                $"{UrlRoot}/specifications-selected-for-funding-by-period?fundingPeriodId={fundingPeriodId}");
        }

        public async Task<HttpStatusCode> SetAssignedTemplateVersion(string specificationId, string templateVersion, string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(templateVersion, nameof(templateVersion));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await PutAsync(
                $"{UrlRoot}/{specificationId}/templates/{fundingStreamId}", templateVersion);
        }

        public async Task<HttpStatusCode> SelectSpecificationForFunding(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PostAsync($"{UrlRoot}/select-for-funding?specificationId={specificationId}");
        }

        public async Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetApprovedSpecifications(string fundingPeriodId, string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<IEnumerable<SpecificationSummary>>($"{UrlRoot}/specifications-by-fundingperiod-and-fundingstream?fundingPeriodId={fundingPeriodId}&fundingStreamId={fundingStreamId}");
        }

        public async Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsSelectedForFunding()
        {
            return await GetAsync<IEnumerable<SpecificationSummary>>($"{UrlRoot}/specifications-selected-for-funding");
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

        public async Task<HttpStatusCode> SetPublishDates(string specificationId, SpecificationPublishDateModel specificationPublishDateModel)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.ArgumentNotNull(specificationPublishDateModel, nameof(specificationPublishDateModel));

            string url = $"{UrlRoot}/{specificationId}/publishdates";
            return await PutAsync(url, specificationPublishDateModel);
        }

        public async Task<ApiResponse<SpecificationPublishDateModel>> GetPublishDates(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            return await GetAsync<SpecificationPublishDateModel>(
                 $"{UrlRoot}/{specificationId}/publishdates");
        }

        public async Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationSummaries()
        {
            return await GetAsync<IEnumerable<SpecificationSummary>>($"{UrlRoot}/specification-summaries");
        }

        public async Task<ValidatedApiResponse<SpecificationSummary>> UpdateSpecification(string specificationId, EditSpecificationModel specification)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.ArgumentNotNull(specification, nameof(specification));

            return await ValidatedPutAsync<SpecificationSummary, EditSpecificationModel>($"{UrlRoot}/specification-edit?specificationId={specificationId}", specification);
        }

        public async Task<ValidatedApiResponse<SpecificationSummary>> CreateSpecification(CreateSpecificationModel specification)
        {
            Guard.ArgumentNotNull(specification, nameof(specification));

            return await ValidatedPostAsync<SpecificationSummary, CreateSpecificationModel>($"{UrlRoot}/specifications", specification);
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

        public async Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecifications(string fundingPeriodId)
        {
            return await GetAsync<IEnumerable<SpecificationSummary>>($"{UrlRoot}/specifications-by-year?fundingPeriodId={fundingPeriodId}");
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

        public async Task<ApiResponse<IEnumerable<string>>> GetFundingStreamIdsForSelectedFundingSpecification()
        {
            return await GetAsync<IEnumerable<string>>($"{UrlRoot}/fundingstream-ids-for-funding-specifications");
        }
    }
}