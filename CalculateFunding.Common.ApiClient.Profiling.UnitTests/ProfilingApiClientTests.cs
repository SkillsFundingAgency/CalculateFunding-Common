using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Profiling.Models;
using CalculateFunding.Common.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;

// ReSharper disable HeapView.CanAvoidClosure

namespace CalculateFunding.Common.ApiClient.ProfilingApiClient.UnitTests
{
    [TestClass]
    public class ProfilingApiClientTests : ApiClientTestBase
    {
        private Profiling.ProfilingApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new Profiling.ProfilingApiClient(ClientFactory,
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
        public async Task CreateProfilePattern()
        {
            await AssertPostRequest("profiling/patterns", 
                new CreateProfilePatternRequest(), 
                HttpStatusCode.OK,
                _client.CreateProfilePattern);
        }
        
        [TestMethod]
        public async Task EditProfilePattern()
        {
            EditProfilePatternRequest request = new EditProfilePatternRequest();
            
            await AssertPutRequest("profiling/patterns", 
                request, 
                HttpStatusCode.OK,
                () => _client.EditProfilePattern(request));
        }

        [TestMethod]
        public async Task DeleteProfilePattern()
        {
            string id = NewRandomString();

            await AssertDeleteRequest($"profiling/patterns/{id}", 
                HttpStatusCode.OK, 
                () => _client.DeleteProfilePattern(id));
        }

        [TestMethod]
        public async Task GetProfilePattern()
        {
            string id = NewRandomString();

            await AssertGetRequest($"profiling/patterns/{id}", 
                new FundingStreamPeriodProfilePattern(),
                () => _client.GetProfilePattern(id));
        }
        
        [TestMethod]
        public async Task GetProfilePatternsForFundingStreamAndFundingPeriod()
        {
            string fundingStreamId = NewRandomString();
            string fundingPeriodId = NewRandomString();

            await AssertGetRequest($"profiling/patterns/fundingStreams/{fundingStreamId}/fundingPeriods/{fundingPeriodId}", 
                Enumerable.Empty<FundingStreamPeriodProfilePattern>(),
                () => _client.GetProfilePatternsForFundingStreamAndFundingPeriod(fundingStreamId, fundingPeriodId));
        }
    }
}