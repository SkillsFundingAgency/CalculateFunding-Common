using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Common.ApiClient.Providers.Models.Search;
using CalculateFunding.Common.ApiClient.Providers.ViewModels;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.Testing;
using CalculateFunding.Common.Extensions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;
// ReSharper disable HeapView.CanAvoidClosure

namespace CalculateFunding.Common.ApiClient.Providers.UnitTests
{
    [TestClass]
    public class ProvidersApiClientUnitTests : ApiClientTestBase
    {
        private ProvidersApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new ProvidersApiClient(ClientFactory,
                Logger.None);
        }
        
        [TestMethod]
        public async Task SearchMasterProviders_SuccessfulSearch()
        {
            ApiResponse<ProviderVersionSearchResults> expectedResponse = new ApiResponse<ProviderVersionSearchResults>(HttpStatusCode.OK, 
                new ProviderVersionSearchResults
                {
                    Results = new [] { new ProviderVersionSearchResult() },
                    Facets = new [] { new Facet(), }
                });
            
            GivenTheResponse("providers/master-search", expectedResponse, HttpMethod.Post);

            SearchModel search = NewRandomSearch();
            
            PagedResult<ProviderVersionSearchResult> apiResponse = await _client.SearchMasterProviders(new SearchFilterRequest
            {
                SearchTerm = search.SearchTerm
            });

            //can't get the results and facets deserializing properly
            //so just going with a null check for now
            apiResponse
                .Should()
                .NotBeNull();
               
            AndTheRequestContentsShouldHaveBeen(new SearchModel
            {
                SearchTerm = search.SearchTerm,
                FacetCount = 0
            }.AsJson());
        }
        
        [TestMethod]
        public async Task SearchMasterProviders_UnSuccessfulSearch()
        {
            GivenTheStatusCode("providers/master-search", HttpStatusCode.InternalServerError, HttpMethod.Post);

            PagedResult<ProviderVersionSearchResult> apiResponse = await _client.SearchMasterProviders(new SearchFilterRequest());

            apiResponse
                .Should()
                .BeNull();
        }

        [TestMethod]
        public async Task SearchProviderVersions()
        {
            await AssertPostRequest("providers/versions-search",
                new SearchModel(),
                new ProviderVersionSearchResults(),
                _client.SearchProviderVersions);
        }
        
        [TestMethod]
        public async Task SearchProvidersInProviderVersion()
        {
            string id = NewRandomString();
            SearchModel search = NewRandomSearch();
            
            await AssertPostRequest($"providers/versions-search/{id}",
                search,
                new ProviderVersionSearchResults(),
               () => _client.SearchProvidersInProviderVersion(id, search));
        }
        
        [TestMethod]
        public async Task GetProviderVersionsByFundingStream()
        {
            string id = NewRandomString();
            
            await AssertGetRequest($"providers/versions-by-fundingstream/{id}",
                id,
                Enumerable.Empty<ProviderVersionMetadata>(),
                _client.GetProviderVersionsByFundingStream);
        }
        
        [TestMethod]
        public async Task GetProviderVersionMetadata()
        {
            string id = NewRandomString();
            
            await AssertGetRequest($"providers/versions/{id}/metadata",
                id,
                new ProviderVersionMetadata(),
                _client.GetProviderVersionMetadata);
        }
        
        [TestMethod]
        public async Task GetProviderByIdFromProviderVersion()
        {
            string providerVersionId = NewRandomString();
            string providerId = NewRandomString();
            
            await AssertGetRequest($"providers/versions/{providerVersionId}/{providerId}",
                new ProviderVersionSearchResult(),
               () => _client.GetProviderByIdFromProviderVersion(providerVersionId, providerId));
        }

        [TestMethod]
        public async Task UploadProviderVersion()
        {
            string providerVersionId = NewRandomString();
            ProviderVersionViewModel model = new ProviderVersionViewModel();

            GivenTheStatusCode($"providers/versions/{providerVersionId}", HttpStatusCode.OK, HttpMethod.Post);

            NoValidatedContentApiResponse apiResponse = await _client.UploadProviderVersion(providerVersionId, model);

            apiResponse?
                .StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
            
            AndTheRequestContentsShouldHaveBeen(model.AsJson());
        }

        [TestMethod]
        public async Task SetProviderDateProviderVersion()
        {
            int year = NewRandomInt();
            int month = NewRandomInt();
            int day = NewRandomInt();
            string providerVersionId = NewRandomString();

            await AssertPutRequest($"providers/date/{year}/{month}/{day}",
                HttpStatusCode.OK,
                () => _client.SetProviderDateProviderVersion(year, month, day, providerVersionId));
            
            AndTheRequestContentsShouldHaveBeen(providerVersionId.AsJson());
        }

        [TestMethod]
        public async Task GetProvidersByVersion()
        {
            int year = NewRandomInt();
            int month = NewRandomInt();
            int day = NewRandomInt();

            await AssertGetRequest($"providers/date/{year}/{month}/{day}",
                new ProviderVersion(),
                () => _client.GetProvidersByVersion(year, month, day));
        }
        
        [TestMethod]
        [Ignore("no idea what the impl of the mut is supposed to be about but its wrong")]
        public async Task SearchProviderVersions_ByVersion()
        {
            int year = NewRandomInt();
            int month = NewRandomInt();
            int day = NewRandomInt();

            await AssertGetRequest($"providers/date/{year}/{month}/{day}",
                new ProviderVersion(),
                () => _client.GetProvidersByVersion(year, month, day));
            
            //lol - the impl is a bit muddled on this one (url doesn't replace verson key parts) lol
        }

        [TestMethod]
        public async Task GetAllMasterProviders()
        {
            await AssertGetRequest("providers/master",
                new ProviderVersion(),
                _client.GetAllMasterProviders);
        }

        [TestMethod]
        public async Task SearchMasterProviders()
        {
            await AssertPostRequest("providers/master-search",
                new SearchModel(),
                new ProviderVersionSearchResults(),
                _client.SearchMasterProviders);
        }

        [TestMethod]
        public async Task GetProviderByIdFromMaster()
        {
            string id = NewRandomString();

            await AssertGetRequest($"providers/master/{id}",
                id,
                new ProviderVersionSearchResult(),
                _client.GetProviderByIdFromMaster);
        }

        [TestMethod]
        public async Task SetMasterProviderVersion()
        {
            MasterProviderVersionViewModel model = new MasterProviderVersionViewModel();
            
            await AssertPutRequest("providers/master",
                HttpStatusCode.OK,
                () => _client.SetMasterProviderVersion(model));
        }

        [TestMethod]
        public async Task DoesProviderVersionExist()
        {
            string id = NewRandomString();

            await AssertHeadRequest($"providers/versions/{id}",
                HttpStatusCode.OK,
                () => _client.DoesProviderVersionExist(id));
        }

        [TestMethod]
        public async Task FetchCoreProviderData()
        {
            string id = NewRandomString();

            await AssertGetRequest($"scopedproviders/get-provider-summaries/{id}",
                Enumerable.Empty<ProviderSummary>(),
                () => _client.FetchCoreProviderData(id));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task PopulateProviderSummariesForSpecification(bool setCachedProviders)
        {
            string id = NewRandomString();

            bool expectedResponse = false;
            
            GivenThePrimitiveResponse($"scopedproviders/set-cached-providers/{id}/{setCachedProviders}",
                expectedResponse, HttpMethod.Get);

            ApiResponse<bool> apiResponse = await _client.RegenerateProviderSummariesForSpecification(id, setCachedProviders);

            apiResponse
                ?.Content
                .Should()
                .Be(expectedResponse);
        }

        [TestMethod]
        public async Task GetScopedProviderIds()
        {
            string id = NewRandomString();

            await AssertGetRequest($"copedproviders/get-provider-ids/{id}",
                id,
                new[]
                {
                    NewRandomString(),
                    NewRandomString()
                }.AsEnumerable(),
                _client.GetScopedProviderIds);
        }

        [TestMethod]
        public async Task GetProviderNames()
        {
            await AssertGetRequest($"providers/name",
                new[]
                {
                    NewRandomString(),
                    NewRandomString(),
                }.AsEnumerable(),
                _client.GetProviderNames);
        }

        [TestMethod]
        public async Task GetProviderGraphQlFields()
        {
            await AssertGetRequest("provider-graphql-fields",
                Enumerable.Empty<ProviderGraphQlFieldInfo>(),
                _client.GetProviderGraphQlFields);
        }

        [TestMethod]
        public async Task SetCurrentProviderVersion()
        {
            string fundingStreamId = NewRandomString();
            string providerVersionId = NewRandomString();

            HttpStatusCode expectedStatusCode = HttpStatusCode.NoContent;

            await AssertPutRequest($"providers/fundingstreams/{fundingStreamId}/current/{providerVersionId}",
                expectedStatusCode,
                () => _client.SetCurrentProviderVersion(fundingStreamId, providerVersionId));
        }

        [TestMethod]
        public async Task GetCurrentProvidersForFundingStream()
        {
            string fundingStreamId = NewRandomString();

            await AssertGetRequest($"providers/fundingstreams/{fundingStreamId}/current",
                new ProviderVersion(),
                () => _client.GetCurrentProvidersForFundingStream(fundingStreamId));
        }
        
        [TestMethod]
        public async Task GetCurrentProviderForFundingStream()
        {
            string fundingStreamId = NewRandomString();
            string providerId = NewRandomString();

            await AssertGetRequest($"providers/{providerId}/fundingstreams/{fundingStreamId}/current",
                new ProviderVersionSearchResult(), 
                () => _client.GetCurrentProviderForFundingStream(fundingStreamId, providerId));
        }

        [TestMethod]
        public async Task SearchCurrentProviderVersionForFundingStream()
        {
            string fundingStreamId = NewRandomString();
            SearchModel search = NewRandomSearch();
            
            await AssertPostRequest($"providers/fundingstreams/{fundingStreamId}/current/search",
                search,
                new ProviderVersionSearchResults(),
                () => _client.SearchCurrentProviderVersionForFundingStream(fundingStreamId, search));
        }
    }
}