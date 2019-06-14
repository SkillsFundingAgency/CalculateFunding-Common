using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Common.ApiClient.Providers.Models.Search;
using CalculateFunding.Common.ApiClient.Providers.ViewModels;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.Utility;
using Serilog;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Providers
{
    public class ProvidersApiClient : BaseApiClient, IProvidersApiClient
    {
        public ProvidersApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
         : base(httpClientFactory, HttpClientKeys.Jobs, logger, cancellationTokenProvider)
        { }

        public async Task<ApiResponse<ProviderVersionSearchResults>> SearchProviderVersions(SearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            string url = "providers/versions-search";

            return await PostAsync<ProviderVersionSearchResults, SearchModel>(url, searchModel);
        }

        public async Task<ApiResponse<ProviderVersionSearchResults>> SearchProvidersInProviderVersion(string providerVersionId, SearchModel searchModel)
        {
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            string url = $"providers/versions-search/{providerVersionId}";

            return await PostAsync<ProviderVersionSearchResults, SearchModel>(url, searchModel);
        }

        public async Task<ApiResponse<ProviderVersion>> GetProvidersByVersion(string providerVersionId)
        {
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));

            string url = $"providers/versions/{providerVersionId}";

            return await GetAsync<ProviderVersion>(url);
        }

        public async Task<ApiResponse<ProviderVersionSearchResults>> GetProviderByIdFromProviderVersion(string providerVersionId, string providerId)
        {
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            string url = $"providers/versions/{providerVersionId}/{providerId}";

            return await GetAsync<ProviderVersionSearchResults>(url);
        }

        public async Task<NoValidatedContentApiResponse> UploadProviderVersion(string providerVersionId, ProviderVersionViewModel providers)
        {
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));
            Guard.ArgumentNotNull(providers, nameof(providers));

            string url = $"providers/versions/{providerVersionId}";

            return await ValidatedPostAsync<ProviderVersionViewModel>(url, providers);
        }

        public async Task<HttpStatusCode> SetProviderDateProviderVersion(int year, int month, int day, string providerVersionId)
        {
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));

            string url = $"providers/date/{year}/{month}/{day}";

            return await PutAsync(url, providerVersionId);
        }

        public async Task<ApiResponse<ProviderVersion>> GetProvidersByVersion(int year, int month, int day)
        {
            string url = $"providers/date/{year}/{month}/{day}";

            return await GetAsync<ProviderVersion>(url);
        }

        public async Task<ApiResponse<ProviderVersionSearchResults>> SearchProviderVersions(int year, int month, int day, SearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            string url = "providers/date-search/{year}/{month}/{day}";

            return await PostAsync<ProviderVersionSearchResults, SearchModel>(url, searchModel);
        }

        public async Task<ApiResponse<ProviderVersion>> GetAllMasterProviders()
        {
            string url = $"providers/master";

            return await GetAsync<ProviderVersion>(url);
        }

        public async Task<ApiResponse<ProviderVersionSearchResults>> SearchMasterProviders(SearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            string url = "providers/master-search";

            return await PostAsync<ProviderVersionSearchResults, SearchModel>(url, searchModel);
        }

        public async Task<ApiResponse<ProviderVersionSearchResults>> GetProviderByIdFromMaster(string providerId)
        {
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            string url = $"providers/master/{providerId}";

            return await GetAsync<ProviderVersionSearchResults>(url);
        }

        public async Task<HttpStatusCode> SetMasterProviderVersion(MasterProviderVersionViewModel masterProviderVersion)
        {
            Guard.ArgumentNotNull(masterProviderVersion, nameof(masterProviderVersion));

            string url = "providers/master";

            return await PutAsync(url, masterProviderVersion);
        }

        public async Task<HttpStatusCode> DoesProviderVersionExist(string providerVersionId)
        {
            Guard.ArgumentNotNull(providerVersionId, nameof(providerVersionId));

            string url = $"providers/versions/{providerVersionId}";

            return await GetAsync(url);
        }
    }
}
