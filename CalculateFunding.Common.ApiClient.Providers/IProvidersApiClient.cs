using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Common.ApiClient.Providers.Models.Search;
using CalculateFunding.Common.ApiClient.Providers.ViewModels;
using CalculateFunding.Common.Models.Search;

namespace CalculateFunding.Common.ApiClient.Providers
{
    public interface IProvidersApiClient
    {
        Task<ApiResponse<ProviderVersionSearchResult>> GetProviderByIdFromProviderVersion(string providerVersionId, string providerId);
        Task<ApiResponse<IEnumerable<ProviderVersionMetadata>>> GetProviderVersionsByFundingStream(string fundingStreamId);
        Task<ApiResponse<ProviderVersionMetadata>> GetProviderVersionMetadata(string providerVersionId);
        Task<ApiResponse<ProviderVersion>> GetProvidersByVersion(int year, int month, int day);
        Task<ApiResponse<ProviderVersion>> GetProvidersByVersion(string providerVersionId);
        Task<ApiResponse<ProviderVersionSearchResults>> SearchProvidersInProviderVersion(string providerVersionId, SearchModel searchModel);
        Task<ApiResponse<ProviderVersionSearchResults>> SearchProviderVersions(int year, int month, int day, SearchModel searchModel);
        Task<ApiResponse<ProviderVersionSearchResults>> SearchProviderVersions(SearchModel searchModel);
        Task<HttpStatusCode> SetProviderDateProviderVersion(int year, int month, int day, string providerVersionId);
        Task<NoValidatedContentApiResponse> UploadProviderVersion(string providerVersionId, ProviderVersionViewModel providers);
        Task<HttpStatusCode> DoesProviderVersionExist(string providerVersionId);
        Task<ApiResponse<IEnumerable<ProviderSummary>>> FetchCoreProviderData(string specificationId);
        Task<ApiResponse<bool>> RegenerateProviderSummariesForSpecification(string specificationId,bool setCachedProviders = false);
        Task<ApiResponse<IEnumerable<string>>> GetScopedProviderIds(string specificationId);
        Task<ApiResponse<IEnumerable<string>>> GetProviderNames();
        Task<ApiResponse<IEnumerable<ProviderGraphQlFieldInfo>>> GetProviderGraphQlFields();

        Task<HttpStatusCode> SetCurrentProviderVersion(string fundingStreamId,
            string providerVersionId);

        Task<ApiResponse<ProviderVersion>> GetCurrentProvidersForFundingStream(string fundingStreamId);

        Task<ApiResponse<ProviderVersionSearchResult>> GetCurrentProviderForFundingStream(string fundingStreamId,
            string providerId);

        Task<ApiResponse<ProviderVersionSearchResults>> SearchCurrentProviderVersionForFundingStream(string fundingStreamId,
            SearchModel search);

        Task<ApiResponse<IEnumerable<string>>> GetLocalAuthorityNamesByProviderVersionId(string providerVersionId);

        Task<ApiResponse<IEnumerable<string>>> GetLocalAuthorityNamesByFundingStreamId(string fundingStreamId);
    }
}
