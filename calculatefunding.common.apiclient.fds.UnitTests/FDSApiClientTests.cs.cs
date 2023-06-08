using System.Threading.Tasks;
using CalculateFunding.Common.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;
using CalculateFunding.Common.ApiClient.FDS;
using CalculateFunding.Common.ApiClient.FDS.Models;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable HeapView.CanAvoidClosure

namespace CalculateFunding.Common.ApiClient.Datasets.UnitTests
{
    [TestClass]
    public class DatasetsApiClientTests : ApiClientTestBase
    {
        private FDSApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new FDSApiClient(ClientFactory,
                Substitute.For<ILogger>());
        }

        [TestMethod]
        public async Task GetDatasetDefinitionsMakesGetCall()
        {
            string id = NewRandomString();

            await AssertGetRequest($"FundingData/schema/{id}",
                id,
                new FDSDatasetDefinition(),
                _ => _client.GetDatasetDefinition(_));
        }

        [TestMethod]
        public async Task GetDatasetForFundingStreamMakesGetCall()
        {
            string fundingStream = NewRandomString();

            await AssertGetRequest($"FundingData/schemas/{fundingStream}",
                fundingStream,
                Enumerable.Empty< DatasetDefinitionByFundingStream >(),
                _ => _client.GetDatasetForFundingStream(_));
        }
    }
}