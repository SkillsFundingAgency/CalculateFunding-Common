using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.CalcEngine.Models;
using CalculateFunding.Common.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;

namespace CalculateFunding.Common.ApiClient.CalcEngine.Tests
{
    [TestClass]
    public class CalcEngineApiClientTests : ApiClientTestBase
    {
        private CalcEngineApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new CalcEngineApiClient(ClientFactory,
                Logger.None);
        }
        
        [TestMethod]
        public async Task PreviewCalculationResultsForSpecificationAndProvider()
        {
            string specificationId = NewRandomString();
            string providerId = NewRandomString();
            string assemblyContentString = NewRandomString();

            PreviewCalculationRequest previewCalculationRequest = new PreviewCalculationRequest();

            await AssertPostRequest($"calculations-results/{specificationId}/{providerId}/preview",
                previewCalculationRequest,
                new ProviderResult(),
                 () => _client.PreviewCalculationResults(specificationId, providerId, previewCalculationRequest));
        }
    }
}