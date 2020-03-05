using System.Collections.Concurrent;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.TestEngine.Models;
using CalculateFunding.Common.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;
// ReSharper disable HeapView.CanAvoidClosure

namespace CalculateFunding.Common.ApiClient.TestEngine.UnitTests
{
    [TestClass]
    public class TestEngineApiTests : ApiClientTestBase
    {
        private TestEngineApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new TestEngineApiClient(ClientFactory,
                Logger.None);
        }
        
        [TestMethod]
        public async Task ValidateGherkin()
        {
            await AssertPostRequest("validate-test",
                NewRandomString(),
                NewRandomString(),
                _client.ValidateGherkin);
        }

        [TestMethod]
        public async Task SearchTestScenarioResults()
        {
            await AssertPostRequest("testscenario-search",
                NewRandomString(),
                NewRandomString(),
                _client.SearchTestScenarioResults);
        }
        
        [TestMethod]
        public async Task Tests()
        {
            await AssertPostRequest("run-tests",
                NewRandomString(),
                NewRandomString(),
                _client.Tests);
        }

        [TestMethod]
        public async Task ResultCounts()
        {
            await AssertPostRequest("get-result-counts",
                NewRandomString(),
                new ConcurrentBag<TestScenarioResultCounts>(),
                _client.ResultCounts);  
        }
        
        [TestMethod]
        public async Task ProviderStatusCountsForTestScenario()
        {
            string id = NewRandomString();
            
            await AssertGetRequest($"get-testscenario-result-counts-for-provider?providerId={id}",
                new ProviderTestScenarioResultCounts(),
                () => _client.ProviderStatusCountsForTestScenario(id));
        }

        [TestMethod]
        public async Task Reindex()
        {
            await AssertGetRequest("testscenario-reindex",
                NewRandomString(),
                _client.Reindex);
        }
        
        [TestMethod]
        public async Task TestScenarioCountsForSpecifications()
        {
            string json = NewRandomString();
            
            await AssertPostRequest($"get-testscenario-result-counts-for-specifications",
                json,
                new ConcurrentBag<SpecificationTestScenarioResultCounts>(), 
                _client.TestScenarioCountsForSpecifications);
        }
     
        [TestMethod]
        public async Task TestScenarioCountsForProviderForSpecification()
        {
            string id = NewRandomString();
            
            await AssertGetRequest($"get-testscenario-result-counts-for-specification-for-provider?providerId={id}",
                new ScenarioResultCounts(), 
                () => _client.TestScenarioCountsForProviderForSpecification(id));
        }
    }
}