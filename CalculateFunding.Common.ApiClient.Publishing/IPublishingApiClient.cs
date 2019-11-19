using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Publishing.Models;
using CalculateFunding.Common.Models.Search;

namespace CalculateFunding.Common.ApiClient.Publishing
{
    public interface IPublishingApiClient
    {
        Task<ApiResponse<PublishedProviderVersion>> GetPublishedProviderVersion(string fundingStreamId, string fundingPeriodId, string providerId, string version);

        Task<ApiResponse<string>> GetPublishedProviderVersionBody(string publishedProviderVersionId);

        Task<ApiResponse<SpecificationCheckChooseForFundingResult>> CanChooseForFunding(string specificationId);

        Task<ValidatedApiResponse<JobCreationResponse>> RefreshFundingForSpecification(string specificationId);

        Task<ValidatedApiResponse<JobCreationResponse>> ApproveFundingForSpecification(string specificationId);

        Task<ValidatedApiResponse<JobCreationResponse>> PublishFundingForSpecification(string specificationId);

        Task<ApiResponse<SearchResults<PublishedProviderSearchItem>>> SearchPublishedProvider(SearchModel searchModel);

        Task<ApiResponse<ProviderFundingStreamStatusResponse>> GetProviderStatusCounts(string specificationId);
    }
}
