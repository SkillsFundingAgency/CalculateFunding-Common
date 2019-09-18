using System.Collections.Concurrent;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.TestEngine.Models;

namespace CalculateFunding.Common.ApiClient.TestEngine
{
    public interface ITestEngineApiClient
    {
        Task<ApiResponse<string>> ValidateGherkin(string gherkinRequestModelJson);
        Task<ApiResponse<string>> SearchTestScenarioResults(string searchModelJson);
        Task<ApiResponse<string>> Tests(string testExecutionModelJson);
        Task<ApiResponse<ConcurrentBag<TestScenarioResultCounts>>> ResultCounts(string testScenariosResultsCountsRequestModelJson);
        Task<ApiResponse<ProviderTestScenarioResultCounts>> ProviderStatusCountsForTestScenario(string providerId);
        Task<ApiResponse<string>> Reindex();
        Task<ApiResponse<ConcurrentBag<SpecificationTestScenarioResultCounts>>> TestScenarioCountsForSpecifications(string specificationsListModelJson);
        Task<ApiResponse<ScenarioResultCounts>> TestScenarioCountsForProviderForSpecification(string providerId);
    }
}