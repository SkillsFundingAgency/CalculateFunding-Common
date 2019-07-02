using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Common.ApiClient.Providers.Models.Search;
using CalculateFunding.Common.ApiClient.Providers.ViewModels;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.Utility;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Providers
{
    public class ProvidersApiClient : BaseApiClient, IProvidersApiClient
    {
        public ProvidersApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
         : base(httpClientFactory, HttpClientKeys.Providers, logger, cancellationTokenProvider)
        { }

        public async Task<PagedResult<ProviderVersionSearchResult>> SearchMasterProviders(SearchFilterRequest filterOptions)
        {
            Guard.ArgumentNotNull(filterOptions, nameof(filterOptions));

            SearchQueryRequest request = SearchQueryRequest.FromSearchFilterRequest(filterOptions);

            SearchModel searchModel = new SearchModel
            {
                PageNumber = filterOptions.Page,
                Top = filterOptions.PageSize,
                SearchTerm = filterOptions.SearchTerm,
                IncludeFacets = filterOptions.IncludeFacets,
                Filters = filterOptions.Filters ?? new Dictionary<string, string[]>(),
                FacetCount = filterOptions.FacetCount,
                SearchMode = filterOptions.SearchMode == ApiClient.Models.SearchMode.All ? Common.Models.Search.SearchMode.All : Common.Models.Search.SearchMode.Any,
                ErrorToggle = filterOptions.ErrorToggle
            };

            ApiResponse<ProviderVersionSearchResults> results = await SearchMasterProviders(searchModel);

            if (results.StatusCode == HttpStatusCode.OK)
            {
                PagedResult<ProviderVersionSearchResult> result = new SearchPagedResult<ProviderVersionSearchResult>(filterOptions, results.Content.TotalCount)
                {
                    Items = results.Content.Results,
                    Facets = results.Content.Facets.Select(x => new SearchFacet
                    {
                        Name = x.Name,
                        FacetValues = x.FacetValues.Select(v => new SearchFacetValue { Name = v.Name, Count = v.Count})
                    }),
                };

                return result;
            }
            else
            {
                return null;
            }
        }

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

        public async Task<ApiResponse<ProviderVersionSearchResult>> GetProviderByIdFromProviderVersion(string providerVersionId, string providerId)
        {
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            string url = $"providers/versions/{providerVersionId}/{providerId}";

            return await GetAsync<ProviderVersionSearchResult>(url);
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

        public async Task<ApiResponse<ProviderVersionSearchResult>> GetProviderByIdFromMaster(string providerId)
        {
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            string url = $"providers/master/{providerId}";

            return await GetAsync<ProviderVersionSearchResult>(url);
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

        public async Task<ApiResponse<IEnumerable<ProviderSummary>>> FetchCoreProviderData(string specificationId)
        {
            Guard.ArgumentNotNull(specificationId, nameof(specificationId));

            string url = $"scopedproviders/get-provider-summaries/{specificationId}";

            return await GetAsync<IEnumerable<ProviderSummary>>(url);
        }

        public async Task<ApiResponse<int?>> PopulateProviderSummariesForSpecification(string specificationId)
        {
            Guard.ArgumentNotNull(specificationId, nameof(specificationId));

            string url = $"scopedproviders/set-cached-providers/{specificationId}";

            return await GetAsync<int?>(url);
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetScopedProviderIds(string specificationId)
        {
            Guard.ArgumentNotNull(specificationId, nameof(specificationId));

            string url = $"scopedproviders/get-provider-ids/{specificationId}";

            return await GetAsync<IEnumerable<string>>(url);
        }
    }
}
