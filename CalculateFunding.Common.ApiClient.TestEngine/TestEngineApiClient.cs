using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.TestEngine.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.TestEngine
{
    public class TestEngineApiClient : BaseApiClient, ITestEngineApiClient
    {
        private const string UrlRoot = "tests";

        public TestEngineApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
            : base(httpClientFactory, HttpClientKeys.Results, logger, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<string>> ValidateGherkin(string gherkinRequestModelJson)
        {
            Guard.IsNullOrWhiteSpace(gherkinRequestModelJson, nameof(gherkinRequestModelJson));

            return await PostAsync<string, string>($"{UrlRoot}/validate-test", gherkinRequestModelJson);
        }

        public async Task<ApiResponse<string>> SearchTestScenarioResults(string searchModelJson)
        {
            Guard.IsNullOrWhiteSpace(searchModelJson, nameof(searchModelJson));

            return await PostAsync<string, string>($"{UrlRoot}/testscenario-search", searchModelJson);
        }

        public async Task<ApiResponse<string>> Tests(string testExecutionModelJson)
        {
            Guard.IsNullOrWhiteSpace(testExecutionModelJson, nameof(testExecutionModelJson));

            return await PostAsync<string, string>($"{UrlRoot}/run-tests", testExecutionModelJson);
        }

        public async Task<ApiResponse<ConcurrentBag<TestScenarioResultCounts>>> ResultCounts(string testScenariosResultsCountsRequestModelJson)
        {
            Guard.IsNullOrWhiteSpace(testScenariosResultsCountsRequestModelJson, nameof(testScenariosResultsCountsRequestModelJson));

            return await PostAsync<ConcurrentBag<TestScenarioResultCounts>, string>($"{UrlRoot}/get-result-counts", testScenariosResultsCountsRequestModelJson);
        }

        public async Task<ApiResponse<ProviderTestScenarioResultCounts>> ProviderStatusCountsForTestScenario(string providerId)
        {
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            return await GetAsync<ProviderTestScenarioResultCounts>($"{UrlRoot}/get-testscenario-result-counts-for-provider?providerId={providerId}");
        }

        public async Task<ApiResponse<string>> Reindex()
        {
            return await GetAsync<string>($"{UrlRoot}/testscenario-reindex");
        }

        public async Task<ApiResponse<ConcurrentBag<SpecificationTestScenarioResultCounts>>> TestScenarioCountsForSpecifications(string specificationsListModelJson)
        {
            Guard.IsNullOrWhiteSpace(specificationsListModelJson, nameof(specificationsListModelJson));

            return await PostAsync<ConcurrentBag<SpecificationTestScenarioResultCounts>, string>($"{UrlRoot}/get-testscenario-result-counts-for-specifications",
                specificationsListModelJson);
        }

        public async Task<ApiResponse<ScenarioResultCounts>> TestScenarioCountsForProviderForSpecification(string providerId)
        {
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            return await GetAsync<ScenarioResultCounts>($"{UrlRoot}/get-testscenario-result-counts-for-specification-for-provider?providerId={providerId}");
        }
    }
}