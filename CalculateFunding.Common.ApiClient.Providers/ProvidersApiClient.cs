using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Common.ApiClient.Providers.Models.Search;
using CalculateFunding.Common.ApiClient.Providers.ViewModels;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.Utility;
using Serilog;
using SearchMode = CalculateFunding.Common.Models.Search.SearchMode;

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
                        FacetValues = x.FacetValues.Select(v => new SearchFacetValue { Name = v.Name, Count = v.Count })
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

            return await PostAsync<ProviderVersionSearchResults, SearchModel>("providers/versions-search", searchModel);
        }

        public async Task<ApiResponse<ProviderVersionSearchResults>> SearchProvidersInProviderVersion(string providerVersionId, SearchModel searchModel)
        {
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            return await PostAsync<ProviderVersionSearchResults, SearchModel>($"providers/versions-search/{providerVersionId}", searchModel);
        }

        public async Task<ApiResponse<IEnumerable<ProviderVersionMetadata>>> GetProviderVersionsByFundingStream(string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<IEnumerable<ProviderVersionMetadata>>($"providers/versions-by-fundingstream/{fundingStreamId}");
        }

        public async Task<ApiResponse<ProviderVersionMetadata>> GetProviderVersionMetadata(string providerVersionId)
        {
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));

            return await GetAsync<ProviderVersionMetadata>($"providers/versions/{providerVersionId}/metadata");
        }

        public async Task<ApiResponse<ProviderVersion>> GetProvidersByVersion(string providerVersionId)
        {
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));

            return await GetAsync<ProviderVersion>($"providers/versions/{providerVersionId}");
        }

        public async Task<ApiResponse<ProviderVersionSearchResult>> GetProviderByIdFromProviderVersion(string providerVersionId, string providerId)
        {
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            return await GetAsync<ProviderVersionSearchResult>($"providers/versions/{providerVersionId}/{providerId}");
        }

        public async Task<NoValidatedContentApiResponse> UploadProviderVersion(string providerVersionId, ProviderVersionViewModel providers)
        {
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));
            Guard.ArgumentNotNull(providers, nameof(providers));

            return await ValidatedPostAsync($"providers/versions/{providerVersionId}", providers);
        }

        public async Task<HttpStatusCode> SetProviderDateProviderVersion(int year, int month, int day, string providerVersionId)
        {
            Guard.IsNullOrWhiteSpace(providerVersionId, nameof(providerVersionId));

            return await PutAsync($"providers/date/{year}/{month}/{day}", providerVersionId);
        }

        public async Task<ApiResponse<ProviderVersion>> GetProvidersByVersion(int year, int month, int day)
        {
            return await GetAsync<ProviderVersion>($"providers/date/{year}/{month}/{day}");
        }

        public async Task<ApiResponse<ProviderVersionSearchResults>> SearchProviderVersions(int year, int month, int day, SearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            return await PostAsync<ProviderVersionSearchResults, SearchModel>("providers/date-search/{year}/{month}/{day}", searchModel);
        }

        public async Task<ApiResponse<ProviderVersion>> GetAllMasterProviders()
        {
            return await GetAsync<ProviderVersion>("providers/master");
        }

        public async Task<ApiResponse<ProviderVersionSearchResults>> SearchMasterProviders(SearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));

            return await PostAsync<ProviderVersionSearchResults, SearchModel>("providers/master-search", searchModel);
        }

        public async Task<ApiResponse<ProviderVersionSearchResult>> GetProviderByIdFromMaster(string providerId)
        {
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            return await GetAsync<ProviderVersionSearchResult>($"providers/master/{providerId}");
        }

        public async Task<HttpStatusCode> SetMasterProviderVersion(MasterProviderVersionViewModel masterProviderVersion)
        {
            Guard.ArgumentNotNull(masterProviderVersion, nameof(masterProviderVersion));

            return await PutAsync("providers/master", masterProviderVersion);
        }

        public async Task<HttpStatusCode> DoesProviderVersionExist(string providerVersionId)
        {
            Guard.ArgumentNotNull(providerVersionId, nameof(providerVersionId));

            return await HeadAsync($"providers/versions/{providerVersionId}");
        }

        public async Task<ApiResponse<IEnumerable<ProviderSummary>>> FetchCoreProviderData(string specificationId)
        {
            Guard.ArgumentNotNull(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<ProviderSummary>>($"scopedproviders/get-provider-summaries/{specificationId}");
        }

        public async Task<ApiResponse<bool>> RegenerateProviderSummariesForSpecification(string specificationId,bool setCachedProviders = false)
        {
            Guard.ArgumentNotNull(specificationId, nameof(specificationId));

            return await GetAsync<bool>($"scopedproviders/set-cached-providers/{specificationId}/{setCachedProviders}");
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetScopedProviderIds(string specificationId)
        {
            Guard.ArgumentNotNull(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<string>>($"scopedproviders/get-provider-ids/{specificationId}");
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetProviderNames()
        {
            return await GetAsync<IEnumerable<string>>("providers/name");
        }
        
        public async Task<ApiResponse<IEnumerable<ProviderGraphQlFieldInfo>>> GetProviderGraphQlFields()
        {
            return await GetAsync<IEnumerable<ProviderGraphQlFieldInfo>>("provider-graphql-fields");
        }
        
        public async Task<HttpStatusCode> SetCurrentProviderVersion(string fundingStreamId,
            string providerVersionId)
        {
            Guard.ArgumentNotNull(fundingStreamId, nameof(fundingStreamId));
            Guard.ArgumentNotNull(providerVersionId, nameof(providerVersionId));

            return await PutAsync($"providers/fundingstreams/{fundingStreamId}/current/{providerVersionId}");
        }

        public async Task<ApiResponse<ProviderVersion>> GetCurrentProvidersForFundingStream(string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<ProviderVersion>($"providers/fundingstreams/{fundingStreamId}/current");
        }
        
        public async Task<ApiResponse<ProviderVersionSearchResult>> GetCurrentProviderForFundingStream(string fundingStreamId,
            string providerId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<ProviderVersionSearchResult>($"providers/{providerId}/fundingstreams/{fundingStreamId}/current");
        }
        
        public async Task<ApiResponse<ProviderVersionSearchResults>> SearchCurrentProviderVersionForFundingStream(string fundingStreamId,
            SearchModel search)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.ArgumentNotNull(search, nameof(search));

            return await PostAsync<ProviderVersionSearchResults, SearchModel>($"providers/fundingstreams/{fundingStreamId}/current/search",
                search);
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetLocalAuthorityNamesByProviderVersionId(string providerVersionId)
        {
            Guard.ArgumentNotNull(providerVersionId, nameof(providerVersionId));

            return await GetAsync<IEnumerable<string>>($"local-authorities/versions/{providerVersionId}");
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetLocalAuthorityNamesByFundingStreamId(string fundingStreamId)
        {
            Guard.ArgumentNotNull(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<IEnumerable<string>>($"local-authorities/fundingstreams/{fundingStreamId}");
        }
    }
}
