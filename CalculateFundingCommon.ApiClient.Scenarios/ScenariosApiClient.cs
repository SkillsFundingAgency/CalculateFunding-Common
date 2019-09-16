using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.Utility;
using CalculateFundingCommon.ApiClient.Scenarios.Models;
using Serilog;

namespace CalculateFundingCommon.ApiClient.Scenarios
{
    public class ScenariosApiClient : BaseApiClient, IScenariosApiClient
    {
        public ScenariosApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null) 
            : base(httpClientFactory, HttpClientKeys.Scenarios, logger, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<CurrentTestScenario>> SaveVersion(CreateNewTestScenarioVersion createNewTestScenarioVersion)
        {
            Guard.ArgumentNotNull(createNewTestScenarioVersion, nameof(createNewTestScenarioVersion));
            
            return await PostAsync<CurrentTestScenario, CreateNewTestScenarioVersion>("save-scenario-test-version", createNewTestScenarioVersion);
        }

        public async Task<ApiResponse<ScenarioSearchResults>> SearchScenarios(SearchModel searchModel)
        {
            Guard.ArgumentNotNull(searchModel, nameof(searchModel));
            
            return await PostAsync<ScenarioSearchResults, SearchModel>("scenarios-search", searchModel);
        }

        public async Task<ApiResponse<IEnumerable<TestScenario>>> GetTestScenariosBySpecificationId(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<IEnumerable<TestScenario>>($"get-scenarios-by-specificationId?specificationId={specificationId}");
        }

        public async Task<ApiResponse<TestScenario>> GetTestScenarioById(string scenarioId)
        {
            Guard.IsNullOrWhiteSpace(scenarioId, nameof(scenarioId));
            
            return await GetAsync<TestScenario>($"get-scenario-by-id?scenarioId={scenarioId}");
        }

        public async Task<HttpStatusCode> ReIndex()
        {
            return await PostAsync("scenarios-search-reindex");
        }

        public async Task<ApiResponse<CurrentTestScenario>> GetCurrentTestScenarioById(string scenarioId)
        {
            Guard.IsNullOrWhiteSpace(scenarioId, nameof(scenarioId));

            return await GetAsync<CurrentTestScenario>($"get-current-scenario-by-id?scenarioId={scenarioId}");
        }
    }
}