﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Publishing.Models;
using CalculateFunding.Common.Models.Search;

namespace CalculateFunding.Common.ApiClient.Publishing
{
    public interface IPublishingApiClient
    {
        Task<ApiResponse<PublishedProviderVersion>> GetPublishedProviderVersion(string fundingStreamId, string fundingPeriodId, string providerId, string version);

        Task<ApiResponse<IEnumerable<PublishedProviderTransaction>>> GetPublishedProviderTransactions(string specificationId, string providerId);

        Task<ApiResponse<string>> GetPublishedProviderVersionBody(string publishedProviderVersionId);

        Task<ApiResponse<SpecificationCheckChooseForFundingResult>> CanChooseForFunding(string specificationId);

        Task<ValidatedApiResponse<IEnumerable<string>>> ValidateSpecificationForRefresh(string specificationId);

        Task<ValidatedApiResponse<JobCreationResponse>> RefreshFundingForSpecification(string specificationId);

        Task<ValidatedApiResponse<JobCreationResponse>> ApproveFundingForSpecification(string specificationId);
        
        Task<ValidatedApiResponse<JobCreationResponse>> ApproveFundingForBatchProviders(string specificationId, ApproveProvidersRequest approveProvidersRequest);

        Task<ValidatedApiResponse<JobCreationResponse>> PublishFundingForSpecification(string specificationId);
        
        Task<ValidatedApiResponse<JobCreationResponse>> PublishFundingForBatchProviders(string specificationId, PublishProvidersRequest publishProvidersRequest);

        Task<ApiResponse<SearchResults<PublishedProviderSearchItem>>> SearchPublishedProvider(SearchModel searchModel);

        Task<ApiResponse<IEnumerable<string>>> SearchPublishedProviderIds(PublishedProviderIdSearchModel searchModel);

        Task<ApiResponse<IEnumerable<ProviderFundingStreamStatusResponse>>> GetProviderStatusCounts(string specificationId, string providerType = null, string localAuthority = null, string status = null);

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

        Task<HttpStatusCode> ApplyCustomProfilePattern(ApplyCustomProfileRequest request);

        Task<ApiResponse<PublishedProviderFundingCount>> GetProviderBatchForReleaseCount(PublishProvidersRequest publishedProviderIds,
            string specificationId);

        Task<ApiResponse<PublishedProviderFundingCount>> GetProviderBatchForApprovalCount(PublishProvidersRequest publishedProviderIds,
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

        Task<ApiResponse<PublishedProviderFundingStructure>> GetPublishedProviderFundingStructure(
           string publishedProviderVersionId, string etag = null);

        Task<ApiResponse<IEnumerable<ProfileTotal>>> PreviewProfileChange(ProfilePreviewRequest request);
    }
}
