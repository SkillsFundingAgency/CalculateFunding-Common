using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.Testing;
using CalculateFundingCommon.ApiClient.Scenarios;
using CalculateFundingCommon.ApiClient.Scenarios.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;

namespace CalculateFunding.Common.ApiClient.Scenarios.UnitTests
{
    [TestClass]
    public class ScenariosApiClientTests : ApiClientTestBase
    {
        private ScenariosApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new ScenariosApiClient(ClientFactory,
                Logger.None);
        }

        [TestMethod]
        public async Task SaveScenarios()
        {
            await AssertPostRequest("save-scenario-test-version",
                new CreateNewTestScenarioVersion(),
                new CurrentTestScenario(),
                _client.SaveVersion);
        }

        [TestMethod]
        public async Task SearchScenarios()
        {
            await AssertPostRequest("scenarios-search",
                NewRandomSearch(),
                new ScenarioSearchResults(),
                _client.SearchScenarios);
        }

        [TestMethod]
        public async Task GetTestScenariosBySpecificationId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-scenarios-by-specificationId?specificationId={id}",
                id,
                Enumerable.Empty<TestScenario>(),
                _client.GetTestScenariosBySpecificationId);
        }
        
        [TestMethod]
        public async Task GetTestScenarioById()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-scenario-by-id?scenarioId={id}",
                id,
                new TestScenario(),
                _client.GetTestScenarioById);
        }

        [TestMethod]
        public async Task ReIndex()
        {
            HttpStatusCode expectedStatusCode = HttpStatusCode.OK;
            
            GivenTheStatusCode("scenarios-search-reindex", expectedStatusCode, HttpMethod.Post);

            HttpStatusCode apiResponse = await _client.ReIndex();

            apiResponse
                .Should()
                .Be(expectedStatusCode);
        }
        
        [TestMethod]
        public async Task GetCurrentTestScenarioById()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-current-scenario-by-id?scenarioId={id}",
                id,
                new CurrentTestScenario(),
                _client.GetCurrentTestScenarioById);
        }
    }
}