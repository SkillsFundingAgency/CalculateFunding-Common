using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Common.ApiClient.Providers.Models.Search;
using CalculateFunding.Common.ApiClient.Providers.ViewModels;
using CalculateFunding.Common.Models.Search;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Providers
{
    public interface IProvidersApiClient
    {
        Task<ApiResponse<ProviderVersion>> GetAllMasterProviders();
        Task<ApiResponse<ProviderVersionSearchResult>> GetProviderByIdFromMaster(string providerId);
        Task<ApiResponse<ProviderVersionSearchResults>> GetProviderByIdFromProviderVersion(string providerVersionId, string providerId);
        Task<ApiResponse<ProviderVersion>> GetProvidersByVersion(int year, int month, int day);
        Task<ApiResponse<ProviderVersion>> GetProvidersByVersion(string providerVersionId);
        Task<ApiResponse<ProviderVersionSearchResults>> SearchMasterProviders(SearchModel searchModel);
        Task<ApiResponse<ProviderVersionSearchResults>> SearchProvidersInProviderVersion(string providerVersionId, SearchModel searchModel);
        Task<ApiResponse<ProviderVersionSearchResults>> SearchProviderVersions(int year, int month, int day, SearchModel searchModel);
        Task<ApiResponse<ProviderVersionSearchResults>> SearchProviderVersions(SearchModel searchModel);
        Task<HttpStatusCode> SetMasterProviderVersion(MasterProviderVersionViewModel masterProviderVersion);
        Task<HttpStatusCode> SetProviderDateProviderVersion(int year, int month, int day, string providerVersionId);
        Task<NoValidatedContentApiResponse> UploadProviderVersion(string providerVersionId, ProviderVersionViewModel providers);
        Task<HttpStatusCode> DoesProviderVersionExist(string providerVersionId);
        Task<ApiResponse<IEnumerable<ProviderSummary>>> FetchCoreProviderData(string specificationId);
        Task<ApiResponse<int?>> PopulateProviderSummariesForSpecification(string specificationId);
        Task<ApiResponse<IEnumerable<string>>> GetScopedProviderIds(string specificationId);
    }
}
