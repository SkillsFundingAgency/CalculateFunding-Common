using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Results.Models;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;
// ReSharper disable HeapView.CanAvoidClosure

namespace CalculateFunding.Common.ApiClient.Results.UnitTests
{
    [TestClass]
    public class ResultsApiClientTests : ApiClientTestBase
    {
        private ResultsApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new ResultsApiClient(ClientFactory,
                Logger.None);
        }
        
        [TestMethod]
        public async Task GetProviderSpecifications()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-provider-specs?providerId={id}",
                id,
                Enumerable.Empty<string>(),
                _client.GetProviderSpecifications);
        }

        [TestMethod]
        public async Task GetProviderResults()
        {
            string providerId = NewRandomString();
            string specificationId = NewRandomString();

            await AssertGetRequest($"get-provider-results?providerId={providerId}&specificationId={specificationId}",
                new ProviderResult(),
                () => _client.GetProviderResults(providerId, specificationId));
        }

        [TestMethod]
        public async Task GetProviderResultByCalculationTypeTemplate()
        {
            string providerId = NewRandomString();
            string specificationId = NewRandomString();

            await AssertGetRequest($"specifications/{specificationId}/provider-result-by-calculationtype/{providerId}/template",
                new ProviderResult(),
                () => _client.GetProviderResultByCalculationTypeTemplate(providerId, specificationId));     
        }
        
        [TestMethod]
        public async Task GetProviderResultByCalculationTypeAdditional()
        {
            string providerId = NewRandomString();
            string specificationId = NewRandomString();

            await AssertGetRequest($"specifications/{specificationId}/provider-result-by-calculationtype/{providerId}/additional",
                new ProviderResult(),
                () => _client.GetProviderResultByCalculationTypeAdditional(providerId, specificationId));     
        }
        
        [TestMethod]
        public async Task GetProviderSourceDataSetsByProviderIdAndSpecificationId()
        {
            string providerId = NewRandomString();
            string specificationId = NewRandomString();

            await AssertGetRequest($"get-provider-source-datasets?providerId={providerId}&specificationId={specificationId}",
                Enumerable.Empty<ProviderSourceDataset>(),
                () => _client.GetProviderSourceDataSetsByProviderIdAndSpecificationId(providerId, specificationId));     
        }

        [TestMethod]
        public async Task ReIndexCalculationProviderResults()
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            
            GivenTheStatusCode("reindex-calc-provider-results", expectedStatusCode, HttpMethod.Get);

            HttpStatusCode apiResponse = await _client.ReIndexCalculationProviderResults();

            apiResponse
                .Should()
                .Be(expectedStatusCode);
        }

        [TestMethod]
        public async Task SearchCalculationProviderResults()
        {
            await AssertPostRequest("calculation-provider-results-search",
                NewRandomSearch(),
                new CalculationProviderResultSearchResults(),
                _client.SearchCalculationProviderResults);
        }

        [TestMethod]
        public async Task GetScopedProviderIdsBySpecificationId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-scoped-providerids?specificationId={id}",
                id,
                NewEnumerable(NewRandomString(), NewRandomString()),
                _client.GetScopedProviderIdsBySpecificationId);
        }

        [TestMethod]
        public async Task GetFundingCalculationResultsForSpecifications()
        {
            await AssertPostRequest("get-calculation-result-totals-for-specifications",
                new SpecificationListModel(),
                 NewEnumerable( new FundingCalculationResultsTotals()),
                _client.GetFundingCalculationResultsForSpecifications);
        }

        [TestMethod]
        public async Task GetProviderResultsBySpecificationId()
        {
            string specificationId = NewRandomString();
            string top = NewRandomString();

            await AssertGetRequest($"get-specification-provider-results?specificationId={specificationId}&top={top}",
                NewEnumerable(new ProviderResult()),
                () => _client.GetProviderResultsBySpecificationId(specificationId, top));
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public async Task HasCalculationResults(bool expectedHasResults)
        {
            string id = NewRandomString();
            
            GivenThePrimitiveResponse($"hasCalculationResults/{id}", expectedHasResults, HttpMethod.Get);

            ApiResponse<bool> apiResponse = await _client.HasCalculationResults(id);

            apiResponse?
                .Content
                .Should()
                .Be(expectedHasResults);
        }

        [TestMethod]
        public async Task GetSpecificationIdsForProvider()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-provider-specs?providerId={id}",
                id,
                NewEnumerable(NewRandomString()),
                _client.GetSpecificationIdsForProvider);
        }
    }
}