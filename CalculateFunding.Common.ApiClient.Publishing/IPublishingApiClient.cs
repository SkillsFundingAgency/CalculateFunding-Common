using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Publishing.Models;
using CalculateFunding.Common.Models.Search;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Publishing
{
    public interface IPublishingApiClient
    {
        Task<ApiResponse<PublishedProviderVersion>> GetCurrentPublishedProviderVersion(string specificationId, string fundingStreamId, string providerId);

        Task<ApiResponse<PublishedProviderVersion>> GetPublishedProviderVersion(string fundingStreamId, string fundingPeriodId, string providerId, string version);

        Task<ApiResponse<IEnumerable<ReleasePublishedProviderTransaction>>> GetReleasedPublishedProviderTransactions(string specificationId, string providerId);

        Task<ApiResponse<IEnumerable<PublishedProviderTransaction>>> GetPublishedProviderTransactions(string specificationId, string providerId);

        Task<ApiResponse<string>> GetPublishedProviderVersionBody(string publishedProviderVersionId);

        Task<ApiResponse<SpecificationCheckChooseForFundingResult>> CanChooseForFunding(string specificationId);

        Task<ValidatedApiResponse<IEnumerable<string>>> ValidateSpecificationForRefresh(string specificationId);

        Task<ValidatedApiResponse<JobCreationResponse>> RefreshFundingForSpecification(string specificationId);

        Task<ValidatedApiResponse<JobCreationResponse>> ApproveFundingForSpecification(string specificationId);

        Task<ValidatedApiResponse<JobCreationResponse>> ApproveFundingForBatchProviders(string specificationId, PublishedProviderIdsRequest approveProvidersRequest);

        Task<ValidatedApiResponse<JobCreationResponse>> PublishFundingForSpecification(string specificationId);

        Task<ValidatedApiResponse<JobCreationResponse>> PublishFundingForBatchProviders(string specificationId, PublishedProviderIdsRequest publishProvidersRequest);

        Task<ApiResponse<SearchResults<PublishedProviderSearchItem>>> SearchPublishedProvider(SearchModel searchModel);
        
        Task<ApiResponse<double?>> GetFundingValue(SearchModel searchModel);

        Task<ApiResponse<IEnumerable<string>>> GetPublishedProviderIds(string specificationId);

        Task<ApiResponse<IEnumerable<string>>> SearchPublishedProviderIds(PublishedProviderIdSearchModel searchModel);

        Task<ApiResponse<IEnumerable<ProviderFundingStreamStatusResponse>>> GetProviderStatusCounts(string specificationId,
            string providerType = null,
            string localAuthority = null,
            string status = null,
            bool? isIndicative = null,
            string monthYearOpened = null);

        Task<ApiResponse<IEnumerable<string>>> SearchPublishedProviderLocalAuthorities(string searchText, string fundingStreamId, string fundingPeriodId);

        Task<ApiResponse<IEnumerable<ProfileTotal>>> GetLatestProfileTotals(string fundingStreamId, string fundingPeriodId, string providerId);

        Task<ApiResponse<IDictionary<int, ProfilingVersion>>> GetAllReleasedProfileTotals(string fundingStreamId, string fundingPeriodId, string providerId);
        Task<HttpStatusCode> SavePaymentDates(string paymentDatesCsv, string fundingStreamId, string fundingPeriodId);
        Task<ApiResponse<FundingStreamPaymentDates>> GetPaymentDates(string fundingStreamId, string fundingPeriodId);

        Task<ApiResponse<IEnumerable<ProfileTotal>>> GetProfileHistory(string fundingStreamId,
            string fundingPeriodId,
            string providerId);

        Task<HttpStatusCode> AssignProfilePatternKeyToPublishedProvider(string fundingStreamId,
            string fundingPeriodId,
            string providerId,
            ProfilePatternKey profilePatternKey);

        Task<NoValidatedContentApiResponse> ApplyCustomProfilePattern(ApplyCustomProfileRequest request);

        Task<ApiResponse<PublishedProviderFundingCount>> GetProviderBatchForReleaseCount(PublishedProviderIdsRequest publishedProviderIds,
            string specificationId);

        Task<ApiResponse<PublishedProviderFundingCount>> GetProviderBatchForApprovalCount(PublishedProviderIdsRequest publishedProviderIds,
            string specificationId);

        Task<ApiResponse<IEnumerable<string>>> GetPublishedProviderErrors(string specificationId);

        Task<ApiResponse<FundingLineProfile>> GetFundingLinePublishedProviderDetails(
            string specificationId,
            string providerId,
            string fundingStreamId,
            string fundingLineId);

        Task<ApiResponse<bool>> PreviousProfileExistsForSpecificationForProviderForFundingLine(
            string specificationId,
            string providerId,
            string fundingStreamId,
            string fundingLineCode);

        Task<ApiResponse<IEnumerable<FundingLineChange>>> GetPreviousProfilesForSpecificationForProviderForFundingLine(
            string specificationId,
            string providerId,
            string fundingStreamId,
            string fundingLineCode);

        Task<ApiResponse<IEnumerable<FundingLineProfile>>> GetCurrentProfileConfig(
            string specificationId,
            string providerId,
            string fundingStreamId);

        Task<ApiResponse<PublishedProviderFundingStructure>> GetCurrentPublishedProviderFundingStructure(
            string specificationId,
            string fundingStreamId,
            string providerId,
            string etag = null);

        Task<ApiResponse<PublishedProviderFundingStructure>> GetPublishedProviderFundingStructure(
           string publishedProviderVersionId, string etag = null);

        Task<ApiResponse<IEnumerable<ProfileTotal>>> PreviewProfileChange(ProfilePreviewRequest request);

        Task<ApiResponse<PublishedProviderDataDownload>> GenerateCsvForBatchPublishedProvidersForRelease(
            PublishedProviderIdsRequest providerIds,
            string specificationId);

        Task<ApiResponse<PublishedProviderDataDownload>> GenerateCsvForBatchPublishedProvidersForApproval(
            PublishedProviderIdsRequest providerIds,
            string specificationId);

        Task<ApiResponse<PublishedProviderDataDownload>> GenerateCsvForAllPublishedProvidersForRelease(
            string specificationId);

        Task<ApiResponse<PublishedProviderDataDownload>> GenerateCsvForAllPublishedProvidersForApproval(
            string specificationId);

        Task<ApiResponse<JobCreationResponse>> QueueSpecificationFundingStreamSqlImport(string specificationId,
            string fundingStreamId);

        Task<ApiResponse<JobCreationResponse>> QueueSpecificationFundingStreamReleasedSqlImport(string specificationId,
            string fundingStreamId);

        Task<ApiResponse<LatestPublishedDate>> GetLatestPublishedDate(string fundingStreamId,
            string fundingPeriodId);

        Task<ApiResponse<BatchUploadResponse>> UploadBatch(BatchUploadRequest request);

        Task<ValidatedApiResponse<JobCreationResponse>> QueueBatchUploadValidation(BatchUploadValidationRequest request);
        Task<ApiResponse<IEnumerable<string>>> GetBatchPublishedProviderIds(string batchId);

        Task<ApiResponse<IEnumerable<AvailableVariationPointerFundingLine>>> GetAvailableFundingLineProfilePeriodsForVariationPointers(string specificationId);

        Task<ValidatedApiResponse<JobCreationResponse>> QueueReleaseProviderVersions(string specificationId, ReleaseProvidersToChannelRequest releaseProvidersToChannelRequest);

        Task<ValidatedApiResponse<JobCreationResponse>> QueueRelease(string specificationId, ReleaseProvidersToChannelRequest releaseProvidersToChannelRequest);
        
        Task<ApiResponse<IEnumerable<Channel>>> GetAllChannels();

        Task<ValidatedApiResponse<Channel>> UpsertChannel(ChannelRequest request);

        Task<ApiResponse<JobCreationResponse>> QueueReleaseManagementDataMigrationJob(params string[] fundingStreamIds);

        Task<HttpStatusCode> PopulateReferenceData(params string[] fundingStreamIds);

        Task<ApiResponse<ReleaseFundingPublishedProvidersSummary>> GetApprovedPublishedProvidersReleaseFundingSummary(string specificationId, ReleaseFundingPublishProvidersRequest request);
    }
}
