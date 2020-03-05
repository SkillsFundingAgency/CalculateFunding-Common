using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Profiling.Models;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;

// ReSharper disable once CheckNamespace
namespace CalculateFunding.Common.ApiClient.Profiling.UnitTests
{
    [TestClass]
    public class ProfilingApiClientTests : ApiClientTestBase
    {
        private ProfilingApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new ProfilingApiClient(ClientFactory,
                HttpClientKeys.Profiling,
                Logger.None,
                BearerTokenProvider);
        }

        [TestMethod]
        public async Task GetProviderProfilePeriods()
        {
            await AssertPostRequest("profiling",
                new ProviderProfilingRequestModel(),
                new ProviderProfilingResponseModel(),
                _client.GetProviderProfilePeriods);
        }

        [TestMethod]
        public async Task SaveProfilingConfig()
        {
            SetFundingStreamPeriodProfilePatternRequestModel model = new SetFundingStreamPeriodProfilePatternRequestModel();
            
            GivenTheStatusCode("profiling", HttpStatusCode.OK, HttpMethod.Post);

            NoValidatedContentApiResponse apiResponse = await _client.SaveProfilingConfig(model);

            apiResponse?
                .StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
            
            AndTheRequestContentsShouldHaveBeen(model.AsJson());
        }
    }
}