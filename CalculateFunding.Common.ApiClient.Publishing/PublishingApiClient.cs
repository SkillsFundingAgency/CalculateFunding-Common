using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Publishing.Models;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.Utility;
using Serilog;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

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

        public async Task<ApiResponse<PublishedProviderVersion>> GetCurrentPublishedProviderVersion(string specificationId, string fundingStreamId, string providerId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            string url = $"specifications/{specificationId}/publishedproviderversions/{providerId}/fundingStreams/{fundingStreamId}";

            return await GetAsync<PublishedProviderVersion>(url);
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

        public async Task<ApiResponse<IEnumerable<ReleasePublishedProviderTransaction>>> GetReleasedPublishedProviderTransactions(string specificationId, string providerId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            string url = $"specifications/{specificationId}/provider/{providerId}/publishedprovidertransactions";

            return await GetAsync<IEnumerable<ReleasePublishedProviderTransaction>>(url);
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

        public async Task<ApiResponse<double?>> GetFundingValue(SearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            string url = $"publishedprovider/fundingvalue-search";

            return await PostAsync<double?, SearchModel>(url, searchModel);
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetPublishedProviderIds(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"publishedprovider/publishedprovider-id/{specificationId}";

            return await GetAsync<IEnumerable<string>>(url);
        }

        public async Task<ApiResponse<IEnumerable<string>>> SearchPublishedProviderIds(PublishedProviderIdSearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            string url = $"publishedprovider/publishedprovider-id-search";

            return await PostAsync<IEnumerable<string>, PublishedProviderIdSearchModel>(url, searchModel);
        }

        public async Task<ValidatedApiResponse<IEnumerable<string>>> ValidateSpecificationForRefresh(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await ValidatedPostAsync<IEnumerable<string>, string>(
                $"specifications/{specificationId}/validate-specification-for-refresh",
                specificationId);
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

        public async Task<ValidatedApiResponse<JobCreationResponse>> ApproveFundingForBatchProviders(string specificationId, PublishedProviderIdsRequest approveProvidersRequest)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await ValidatedPostAsync<JobCreationResponse, PublishedProviderIdsRequest>($"specifications/{specificationId}/approve-providers", approveProvidersRequest);
        }

        public async Task<ValidatedApiResponse<JobCreationResponse>> PublishFundingForSpecification(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await ValidatedPostAsync<JobCreationResponse, string>($"specifications/{specificationId}/publish", specificationId);
        }

        public async Task<ValidatedApiResponse<JobCreationResponse>> PublishFundingForBatchProviders(string specificationId, PublishedProviderIdsRequest publishProvidersRequest)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await ValidatedPostAsync<JobCreationResponse, PublishedProviderIdsRequest>($"specifications/{specificationId}/publish-providers", publishProvidersRequest);
        }

        public async Task<ApiResponse<IEnumerable<ProviderFundingStreamStatusResponse>>> GetProviderStatusCounts(string specificationId,
            string providerType = null,
            string localAuthority = null,
            string status = null,
            bool? isIndicative = null,
            string monthYearOpened = null)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            string url = $"specifications/{specificationId}/publishedproviders/publishingstatus?providerType={providerType}&localAuthority={localAuthority}&status={status}&monthYearOpened={monthYearOpened}";

            if (isIndicative.HasValue)
            {
                url += $"&isIndicative={isIndicative.Value}";
            }

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

        public async Task<NoValidatedContentApiResponse> ApplyCustomProfilePattern(ApplyCustomProfileRequest request)
        {
            Guard.ArgumentNotNull(request, nameof(request));

            return await ValidatedPostAsync($"publishedproviders/customprofiles", request);
        }

        public async Task<ApiResponse<PublishedProviderFundingCount>> GetProviderBatchForReleaseCount(
            PublishedProviderIdsRequest publishedProviderIds,
            string specificationId)
        {
            Guard.ArgumentNotNull(publishedProviderIds, nameof(publishedProviderIds));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PostAsync<PublishedProviderFundingCount, PublishedProviderIdsRequest>($"specifications/{specificationId}/publishedproviders/publishingstatus-for-release",
                publishedProviderIds);
        }

        public async Task<ApiResponse<PublishedProviderFundingCount>> GetProviderBatchForApprovalCount(
            PublishedProviderIdsRequest publishedProviderIds,
            string specificationId)
        {
            Guard.ArgumentNotNull(publishedProviderIds, nameof(publishedProviderIds));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PostAsync<PublishedProviderFundingCount, PublishedProviderIdsRequest>($"specifications/{specificationId}/publishedproviders/publishingstatus-for-approval",
                publishedProviderIds);
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetPublishedProviderErrors(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<string>>($"publishedprovidererrors/{specificationId}");
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
                $"publishedproviderfundinglinedetails/{specificationId}/{providerId}/{fundingStreamId}/{fundingLineId}");
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
                $"publishedproviderfundinglinedetails/{specificationId}/{providerId}/{fundingStreamId}/{fundingLineCode}/change-exists");
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
                $"publishedproviderfundinglinedetails/{specificationId}/{providerId}/{fundingStreamId}/{fundingLineCode}/changes");
        }

        public async Task<ApiResponse<IEnumerable<FundingLineProfile>>> GetCurrentProfileConfig(
            string specificationId, string providerId, string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<IEnumerable<FundingLineProfile>>(
                $"publishedproviderfundinglinedetails/{specificationId}/{providerId}/{fundingStreamId}");
        }

        public async Task<ApiResponse<PublishedProviderFundingStructure>> GetCurrentPublishedProviderFundingStructure(
            string specificationId,
            string fundingStreamId,
            string providerId,
            string etag = null)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            return await GetAsync<PublishedProviderFundingStructure>(
                $"specifications/{specificationId}/publishedproviders/{providerId}/fundingStreams/{fundingStreamId}/fundingStructure",
                customHeaders: EtagHeader(etag));
        }

        public async Task<ApiResponse<PublishedProviderFundingStructure>> GetPublishedProviderFundingStructure(
            string publishedProviderVersionId,
            string etag = null)
        {
            Guard.IsNullOrWhiteSpace(publishedProviderVersionId, nameof(publishedProviderVersionId));

            return await GetAsync<PublishedProviderFundingStructure>(
                $"publishedproviderfundingstructure/{publishedProviderVersionId}",
                customHeaders: EtagHeader(etag));
        }

        public async Task<ApiResponse<IEnumerable<ProfileTotal>>> PreviewProfileChange(ProfilePreviewRequest request)
        {
            Guard.ArgumentNotNull(request, nameof(request));

            return await PostAsync<IEnumerable<ProfileTotal>, ProfilePreviewRequest>("publishedproviderfundinglinepreview", request);
        }

        private string[] EtagHeader(string etag)
           => etag.IsNullOrEmpty()
               ? null
               : new[]
               {
                    IfNoneMatch, etag
               };

        public async Task<ApiResponse<PublishedProviderDataDownload>> GenerateCsvForBatchPublishedProvidersForRelease(
            PublishedProviderIdsRequest providerIds,
            string specificationId)
        {
            Guard.ArgumentNotNull(providerIds, nameof(providerIds));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PostAsync<PublishedProviderDataDownload, PublishedProviderIdsRequest>(
                $"specifications/{specificationId}/publishedproviders/generate-csv-for-release/batch",
                providerIds);
        }

        public async Task<ApiResponse<PublishedProviderDataDownload>> GenerateCsvForBatchPublishedProvidersForApproval(
            PublishedProviderIdsRequest providerIds,
            string specificationId)
        {
            Guard.ArgumentNotNull(providerIds, nameof(providerIds));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PostAsync<PublishedProviderDataDownload, PublishedProviderIdsRequest>(
                $"specifications/{specificationId}/publishedproviders/generate-csv-for-approval/batch",
                providerIds);
        }

        public async Task<ApiResponse<PublishedProviderDataDownload>> GenerateCsvForAllPublishedProvidersForRelease(
            string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PostAsync<PublishedProviderDataDownload, string>(
                $"specifications/{specificationId}/publishedproviders/generate-csv-for-release/all",
                string.Empty);
        }

        public async Task<ApiResponse<PublishedProviderDataDownload>> GenerateCsvForAllPublishedProvidersForApproval(
            string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await PostAsync<PublishedProviderDataDownload, string>(
                $"specifications/{specificationId}/publishedproviders/generate-csv-for-approval/all",
                string.Empty);
        }

        public async Task<ApiResponse<JobCreationResponse>> QueueSpecificationFundingStreamSqlImport(string specificationId,
            string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<JobCreationResponse>($"sqlqa/specifications/{specificationId}/funding-streams/{fundingStreamId}/import/queue");
        }

        public async Task<ApiResponse<JobCreationResponse>> QueueSpecificationFundingStreamReleasedSqlImport(string specificationId,
            string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<JobCreationResponse>($"sqlqa/specifications/{specificationId}/funding-streams/{fundingStreamId}/released/import/queue");
        }

        public async Task<ApiResponse<LatestPublishedDate>> GetLatestPublishedDate(string fundingStreamId,
            string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            return await GetAsync<LatestPublishedDate>($"publishedproviders/{fundingStreamId}/{fundingPeriodId}/lastupdated");
        }

        public async Task<ApiResponse<BatchUploadResponse>> UploadBatch(BatchUploadRequest request)
        {
            Guard.ArgumentNotNull(request, nameof(request));

            return await PostAsync<BatchUploadResponse, BatchUploadRequest>("publishedproviderbatch", request);
        }

        public async Task<ValidatedApiResponse<JobCreationResponse>> QueueBatchUploadValidation(BatchUploadValidationRequest request)
        {
            Guard.ArgumentNotNull(request, nameof(request));

            return await ValidatedPostAsync<JobCreationResponse, BatchUploadValidationRequest>("publishedproviderbatch/validate", request);
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetBatchPublishedProviderIds(string batchId)
        {
            Guard.IsNullOrWhiteSpace(batchId, nameof(batchId));

            return await GetAsync<IEnumerable<string>>($"publishedproviderbatch/{batchId}/publishedProviders");
        }

        public async Task<ApiResponse<IEnumerable<AvailableVariationPointerFundingLine>>> GetAvailableFundingLineProfilePeriodsForVariationPointers(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<AvailableVariationPointerFundingLine>>($"specifications/{specificationId}/availablePeriodsForVariationPointers");
        }

        public async Task<ApiResponse<JobCreationResponse>> QueueReleaseProviderVersions(string specificationId, ReleaseProvidersToChannelRequest releaseProvidersToChannelRequest)
        {
            Guard.ArgumentNotNull(specificationId, nameof(specificationId));
            Guard.ArgumentNotNull(releaseProvidersToChannelRequest, nameof(releaseProvidersToChannelRequest));

            return await PostAsync<JobCreationResponse, ReleaseProvidersToChannelRequest>(
                $"specifications/{specificationId}/releaseProvidersToChannels", releaseProvidersToChannelRequest);
        }

        public async Task<ApiResponse<ReleaseFundingPublishedProvidersSummary>> GetApprovedPublishedProvidersReleaseFundingSummary(string specificationId, ReleaseFundingPublishProvidersRequest request)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.ArgumentNotNull(request, nameof(request));

            return await PostAsync<ReleaseFundingPublishedProvidersSummary, ReleaseFundingPublishProvidersRequest>(
                $"specifications/{specificationId}/publishedproviders/release-funding-summary", request);
        }

        public async Task<ApiResponse<JobCreationResponse>> QueueReleaseManagementDataMigrationJob(params string[] fundingStreamIds)
        {
            string queryString = AddQueryStringParametersIfSupplied("", nameof(fundingStreamIds), fundingStreamIds);

            return await GetAsync<JobCreationResponse>(
                $"releasemanagement/queuereleasemanagementdatamigrationjob?{queryString}");
        }

        public async Task<HttpStatusCode> PopulateReferenceData(params string[] fundingStreamIds)
        {
            string queryString = AddQueryStringParametersIfSupplied("", nameof(fundingStreamIds), fundingStreamIds);

            return await GetAsync($"releasemanagement/populatereferencedata?{queryString}");
        }

        public async Task<ApiResponse<IEnumerable<Channel>>> GetAllChannels()
        {
            return await GetAsync<IEnumerable<Channel>>("releasemanagement/channels");
        }

        public async Task<ValidatedApiResponse<Channel>> UpsertChannel(ChannelRequest request)
        {
            Guard.ArgumentNotNull(request, nameof(request));

            return await ValidatedPostAsync<Channel, ChannelRequest>(
                "releasemanagement/channels", request);
        }

        private string AddQueryStringParametersIfSupplied<T>(string queryString, string parameterName, T[] parameters)
        {
            if (parameters.IsNullOrEmpty())
            {
                return queryString;
            }

            string queryStringParameters = string.Join($"&{parameterName}=", parameters);
            string queryStringParameter = $"{parameterName}={queryStringParameters}";

            return queryString.Length == 0 ? queryStringParameter : $"{queryString}&{queryStringParameter}";
        }
    }
}
