using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Results.Models;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Results
{
    public class ResultsApiClient : BaseApiClient, IResultsApiClient
    {
        private const string UrlRoot = "results";

        public ResultsApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
            : base(httpClientFactory, HttpClientKeys.Results, logger, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetProviderSpecifications(string providerId)
        {
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            return await GetAsync<IEnumerable<string>>($"{UrlRoot}/get-provider-specs?providerId={providerId}");
        }

        public async Task<ApiResponse<ProviderResultResponse>> GetProviderResults(string providerId, string specificationId)
        {
            EnsureProviderIdAndSpecificationIdSupplied(providerId, specificationId);

            return await GetAsync<ProviderResultResponse>($"{UrlRoot}/get-provider-results?providerId={providerId}&specificationId={specificationId}");
        }

        public async Task<ApiResponse<ProviderResult>> GetProviderResultByCalculationTypeTemplate(string providerId, string specificationId)
        {
            EnsureProviderIdAndSpecificationIdSupplied(providerId, specificationId);

            return await GetAsync<ProviderResult>($"{UrlRoot}/specifications/{specificationId}/provider-result-by-calculationtype/{providerId}/template");
        }

        public async Task<ApiResponse<ProviderResult>> GetProviderResultByCalculationTypeAdditional(string providerId, string specificationId)
        {
            EnsureProviderIdAndSpecificationIdSupplied(providerId, specificationId);

            return await GetAsync<ProviderResult>($"{UrlRoot}/specifications/{specificationId}/provider-result-by-calculationtype/{providerId}/additional");
        }

        public async Task<ApiResponse<IEnumerable<ProviderSourceDataset>>> GetProviderSourceDataSetsByProviderIdAndSpecificationId(string providerId, string specificationId)
        {
            EnsureProviderIdAndSpecificationIdSupplied(providerId, specificationId);

            return await GetAsync<IEnumerable<ProviderSourceDataset>>(
                $"{UrlRoot}/get-provider-source-datasets?providerId={providerId}&specificationId={specificationId}");
        }

        public async Task<HttpStatusCode> ReIndexCalculationProviderResults()
        {
            return await GetAsync($"{UrlRoot}/reindex-calc-provider-results");
        }

        public async Task<ApiResponse<CalculationProviderResultSearchResults>> SearchCalculationProviderResults(SearchModel search)
        {
            Guard.ArgumentNotNull(search, nameof(search));

            return await PostAsync<CalculationProviderResultSearchResults, SearchModel>($"{UrlRoot}/calculation-provider-results-search", search);
        }

        public async Task<ApiResponse<CalculationProviderResultSearchResults>> SearchFundingLineProviderResults(SearchModel search)
        {
            Guard.ArgumentNotNull(search, nameof(search));

            return await PostAsync<CalculationProviderResultSearchResults, SearchModel>($"{UrlRoot}/funding-line-provider-results-search", search);
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetScopedProviderIdsBySpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<string>>($"{UrlRoot}/get-scoped-providerids?specificationId={specificationId}");
        }

        public async Task<ApiResponse<IEnumerable<FundingCalculationResultsTotals>>> GetFundingCalculationResultsForSpecifications(SpecificationListModel specificationList)
        {
            Guard.ArgumentNotNull(specificationList, nameof(specificationList));

            return await PostAsync<IEnumerable<FundingCalculationResultsTotals>, SpecificationListModel>(
                $"{UrlRoot}/get-calculation-result-totals-for-specifications", specificationList);
        }

        public async Task<ApiResponse<IEnumerable<ProviderResult>>> GetProviderResultsBySpecificationId(string specificationId, string top = null)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<ProviderResult>>($"{UrlRoot}/get-specification-provider-results?specificationId={specificationId}&top={top}");
        }

        public async Task<ApiResponse<bool>> HasCalculationResults(string calculationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));

            return await GetAsync<bool>($"{UrlRoot}/hasCalculationResults/{calculationId}");
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetSpecificationIdsForProvider(string providerId)
        {
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            return await GetAsync<IEnumerable<string>>($"{UrlRoot}/get-provider-specs?providerId={providerId}");
        }

        public async Task<ApiResponse<bool>> GetProviderHasResultsBySpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<bool>($"{UrlRoot}/provider-has-results?specificationId={specificationId}");
        }

        public async Task<ApiResponse<IEnumerable<SpecificationInformation>>> GetSpecificationsWithProviderResultsForProviderId(string providerId)
        {
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            return await GetAsync<IEnumerable<SpecificationInformation>>($"{UrlRoot}/providers/{providerId}/specifications");
        }

        public async Task<HttpStatusCode> QueueMergeSpecificationInformationJob(MergeSpecificationInformationRequest mergeRequest)
        {
            Guard.ArgumentNotNull(mergeRequest, nameof(mergeRequest));
            
            return await PutAsync($"{UrlRoot}/providers/specifications", mergeRequest);
        }

        public async Task<NoValidatedContentApiResponse> UpdateFundingStructureLastModified(UpdateFundingStructureLastModifiedRequest request)
        {
            Guard.ArgumentNotNull(request, nameof(request));

            return await ValidatedPostAsync("funding-structures/lastModified", request);
        }

        public async Task<ApiResponse<FundingStructure>> GetFundingStructure(string fundingStreamId,
            string fundingPeriodId,
            string specificationId,
            string etag = null)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<FundingStructure>(
                $"funding-structures?fundingStreamId={fundingStreamId}&fundingPeriodId={fundingPeriodId}&specificationId={specificationId}",
                customHeaders: EtagHeader(etag));
        }

        public async Task<ApiResponse<FundingStructure>> GetFundingStructureResults(string fundingStreamId,
            string fundingPeriodId,
            string specificationId,
            string providerId = null,
            string etag = null)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"funding-structures/results?fundingStreamId={fundingStreamId}&fundingPeriodId={fundingPeriodId}&specificationId={specificationId}";

            url = providerId.IsNullOrEmpty() ? url : $"{url}&providerId={providerId}";

            return await GetAsync<FundingStructure>(
                url,
                customHeaders: EtagHeader(etag));
        }

        private string[] EtagHeader(string etag)
            => etag.IsNullOrEmpty()
                ? null
                : new[]
                {
                    IfNoneMatch, etag
                };

        private void EnsureProviderIdAndSpecificationIdSupplied(string providerId, string specificationId)
        {
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
        }
    }
}