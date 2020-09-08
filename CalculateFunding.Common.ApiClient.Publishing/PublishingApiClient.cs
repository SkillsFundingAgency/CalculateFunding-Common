using System.Collections.Generic;
using System.Net;
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

        public async Task<ApiResponse<IEnumerable<ProfileTotal>>> GetProfileHistory(string fundingStreamId,
            string fundingPeriodId,
            string providerId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            return await GetAsync<IEnumerable<ProfileTotal>>(
                $"fundingstreams/{fundingStreamId}/fundingperiods/{fundingPeriodId}/providers/{providerId}/profilinghistory");
        }

        public async Task<HttpStatusCode> SavePaymentDates(string paymentDatesCsv, string fundingStreamId, string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(paymentDatesCsv, nameof(paymentDatesCsv));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            
            return await PostAsync($"fundingstreams/{fundingStreamId}/fundingperiods/{fundingPeriodId}/paymentdates", 
                default, 
                paymentDatesCsv);
        }
        
        public async Task<ApiResponse<FundingStreamPaymentDates>> GetPaymentDates(string fundingStreamId, string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            
            return await GetAsync<FundingStreamPaymentDates>($"fundingstreams/{fundingStreamId}/fundingperiods/{fundingPeriodId}/paymentdates");
        }
        
        public async Task<ApiResponse<IEnumerable<ProfileTotal>>> GetLatestProfileTotals(string fundingStreamId, string fundingPeriodId, string providerId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));
            
            string url = $"publishedproviders/{fundingStreamId}/{fundingPeriodId}/{providerId}/profileTotals";

            return await GetAsync<IEnumerable<ProfileTotal>>(url);
        }

        public async Task<ApiResponse<IDictionary<int, ProfilingVersion>>> GetAllReleasedProfileTotals(string fundingStreamId, string fundingPeriodId, string providerId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            string url = $"publishedproviders/{fundingStreamId}/{fundingPeriodId}/{providerId}/allProfileTotals";

            return await GetAsync<IDictionary<int, ProfilingVersion>>(url);
        }

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

        public async Task<ApiResponse<IEnumerable<string>>> SearchPublishedProviderIds(PublishedProviderIdSearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            string url = $"publishedprovider/publishedprovider-id-search";

            return await PostAsync<IEnumerable<string>, PublishedProviderIdSearchModel>(url, searchModel);
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

        public async Task<ValidatedApiResponse<JobCreationResponse>> ApproveFundingForBatchProviders(string specificationId, ApproveProvidersRequest approveProvidersRequest)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await ValidatedPostAsync<JobCreationResponse, ApproveProvidersRequest>($"specifications/{specificationId}/approve-providers", approveProvidersRequest);
        }

        public async Task<ValidatedApiResponse<JobCreationResponse>> PublishFundingForSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await ValidatedPostAsync<JobCreationResponse, string>($"specifications/{specificationId}/publish", specificationId);
        }

        public async Task<ValidatedApiResponse<JobCreationResponse>> PublishFundingForBatchProviders(string specificationId, PublishProvidersRequest publishProvidersRequest)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await ValidatedPostAsync<JobCreationResponse, PublishProvidersRequest>($"specifications/{specificationId}/publish-providers", publishProvidersRequest);
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

        public async Task<HttpStatusCode> AssignProfilePatternKeyToPublishedProvider(string fundingStreamId, string fundingPeriodId, string providerId, ProfilePatternKey profilePatternKey)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            string url = $"publishedprovider/fundingStream/{fundingStreamId}/fundingPeriod/{fundingPeriodId}/provider/{providerId}";

            return await PostAsync(url, profilePatternKey);
        }

        public async Task<ValidatedApiResponse<HttpStatusCode>> ApplyCustomProfilePattern(ApplyCustomProfileRequest request)
        {
            Guard.ArgumentNotNull(request, nameof(request));

            return await ValidatedPostAsync<HttpStatusCode, ApplyCustomProfileRequest>($"publishedproviders/customprofiles", request);
        }

        public async Task<ApiResponse<PublishedProviderFundingCount>> GetProviderBatchForReleaseCount(PublishProvidersRequest publishedProviderIds,
            string specificationId)
        {
            Guard.ArgumentNotNull(publishedProviderIds, nameof(publishedProviderIds));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PostAsync<PublishedProviderFundingCount, PublishProvidersRequest>($"specifications/{specificationId}/publishedproviders/publishingstatus-for-release",
                publishedProviderIds);
        }

        public async Task<ApiResponse<PublishedProviderFundingCount>> GetProviderBatchForApprovalCount(PublishProvidersRequest publishedProviderIds,
            string specificationId)
        {
            Guard.ArgumentNotNull(publishedProviderIds, nameof(publishedProviderIds));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PostAsync<PublishedProviderFundingCount, PublishProvidersRequest>($"specifications/{specificationId}/publishedproviders/publishingstatus-for-approval",
                publishedProviderIds);
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetPublishedProviderErrors(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<string>>($"api/publishedprovidererrors/{specificationId}");
        }

        public async Task<ApiResponse<FundingLineProfile>> GetFundingLinePublishedProviderDetails(
            string specificationId, 
            string providerId, 
            string fundingStreamId, 
            string fundingLineId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingLineId, nameof(fundingLineId));

            return await GetAsync<FundingLineProfile>(
                $"api/publishedproviderfundinglinedetails/{specificationId}/{providerId}/{fundingStreamId}/{fundingLineId}");
        }

        public async Task<ApiResponse<bool>> PreviousProfileExistsForSpecificationForProviderForFundingLine(
            string specificationId, 
            string providerId, 
            string fundingStreamId, 
            string fundingLineCode)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingLineCode, nameof(fundingLineCode));

            return await GetAsync<bool>(
                $"api/publishedproviderfundinglinedetails/{specificationId}/{providerId}/{fundingStreamId}/{fundingLineCode}/change-exists");
        }

        public async Task<ApiResponse<IEnumerable<FundingLineChange>>> GetPreviousProfilesForSpecificationForProviderForFundingLine(
            string specificationId, 
            string providerId, 
            string fundingStreamId, 
            string fundingLineCode)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingLineCode, nameof(fundingLineCode));

            return await GetAsync<IEnumerable<FundingLineChange>>(
                $"api/publishedproviderfundinglinedetails/{specificationId}/{providerId}/{fundingStreamId}/{fundingLineCode}/changes");
        }
    }
}
