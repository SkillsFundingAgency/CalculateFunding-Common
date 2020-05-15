using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Models.Search;
using CalculateFunding.Common.ApiClient.Scenarios.Models;

namespace CalculateFunding.Common.ApiClient.Scenarios
{
    public interface IScenariosApiClient
    {
        Task<ApiResponse<CurrentTestScenario>> SaveVersion(CreateNewTestScenarioVersion createNewTestScenarioVersion);
        Task<ApiResponse<ScenarioSearchResults>> SearchScenarios(SearchModel searchModel);
        Task<ApiResponse<IEnumerable<TestScenario>>> GetTestScenariosBySpecificationId(string specificationId);
        Task<ApiResponse<TestScenario>> GetTestScenarioById(string scenarioId);
        Task<HttpStatusCode> ReIndex();
        Task<ApiResponse<CurrentTestScenario>> GetCurrentTestScenarioById(string scenarioId);
    }
}