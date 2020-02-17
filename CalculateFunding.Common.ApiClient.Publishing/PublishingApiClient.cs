using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Publishing.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Publishing
{
    public class PublishingApiClient : BaseApiClient, IPublishingApiClient
    {
        public PublishingApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
        : base(httpClientFactory, HttpClientKeys.Publishing, logger, cancellationTokenProvider)
        { }

        public async Task<ApiResponse<PublishedProviderVersion>> GetPublishedProviderVersion(string fundingStreamId, string fundingPeriodId, string providerId, string version)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));
            Guard.IsNullOrWhiteSpace(version, nameof(version));

            string url = $"publishedproviderversions/{fundingStreamId}/{fundingPeriodId}/{providerId}/{version}";

            return await GetAsync<PublishedProviderVersion>(url);
        }

        public async Task<ApiResponse<IEnumerable<PublishedProviderTransaction>>> GetPublishedProviderTransactions(string specificationId, string providerId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            string url = $"publishedprovidertransactions/{specificationId}/{providerId}";

            return await GetAsync<IEnumerable<PublishedProviderTransaction>>(url);
        }

        public async Task<ApiResponse<string>> GetPublishedProviderVersionBody(string publishedProviderVersionId)
        {
            Guard.IsNullOrWhiteSpace(publishedProviderVersionId, nameof(publishedProviderVersionId));

            string url = $"publishedproviderversion/{publishedProviderVersionId}/body";

            return await GetAsync<string>(url);
        }

        public async Task<ApiResponse<SpecificationCheckChooseForFundingResult>> CanChooseForFunding(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"specifications/{specificationId}/funding/canChoose";

            return await GetAsync<SpecificationCheckChooseForFundingResult>(url);
        }

        public async Task<ApiResponse<SearchResults<PublishedProviderSearchItem>>> SearchPublishedProvider(SearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            string url = $"publishedprovider/publishedprovider-search";

            return await PostAsync<SearchResults<PublishedProviderSearchItem>, SearchModel>(url, searchModel);

        }

        public async Task<ValidatedApiResponse<JobCreationResponse>> RefreshFundingForSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await ValidatedPostAsync<JobCreationResponse, string>($"specifications/{specificationId}/refresh", specificationId);
        }

        public async Task<ValidatedApiResponse<JobCreationResponse>> ApproveFundingForSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await ValidatedPostAsync<JobCreationResponse, string>($"specifications/{specificationId}/approve", specificationId);
        }

        public async Task<ValidatedApiResponse<JobCreationResponse>> PublishFundingForSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await ValidatedPostAsync<JobCreationResponse, string>($"specifications/{specificationId}/publish", specificationId);
        }

        public async Task<ApiResponse<IEnumerable<ProviderFundingStreamStatusResponse>>> GetProviderStatusCounts(string specificationId, string providerType = null, string localAuthority = null, string status = null)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"specifications/{specificationId}/publishedproviders/publishingstatus?providerType={providerType}&localAuthority={localAuthority}&status={status}";

            return await GetAsync<IEnumerable<ProviderFundingStreamStatusResponse>>(url);
        }

        public async Task<ApiResponse<IEnumerable<string>>> SearchPublishedProviderLocalAuthorities(string searchText, string fundingStreamId, string fundingPeriodId)
        {
            string url = $"publishedproviders/{fundingStreamId}/{fundingPeriodId}/localauthorities?searchText={searchText}";

            return await GetAsync<IEnumerable<string>>(url);
        }
    }
}
