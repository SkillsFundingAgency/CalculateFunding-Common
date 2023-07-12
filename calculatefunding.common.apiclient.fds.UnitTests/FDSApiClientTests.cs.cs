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
        public async Task GetFDSDataSchemaMakesGetCall()
        {
            string fundingStream = NewRandomString();
            string fundingPeriod = NewRandomString();
            DataSchemaRequest request = new DataSchemaRequest()
            {
                FundingPeriodCode = fundingPeriod,
                FundingStreamCode = fundingStream
            };

            await AssertPostRequest($"FundingData/schema/versions/query",
                request,
                Enumerable.Empty< DatasetDefinitionByFundingStream >(),
                _ => _client.GetFDSDataSchema(fundingStream, fundingPeriod));
        }

        [TestMethod]
        public async Task GetDatasetVersionsByDefinitionIdMakesGetCall()
        {
            string id = NewRandomString();

            await AssertGetRequest($"FundingData/FundingDataVersions/Schema/{id}",
                id,
                Enumerable.Empty<FDSDatasetVersion>(),
                _ => _client.GetDatasetVersionsByDefinitionId(_));
        }

        [TestMethod]
        public async Task GetDatasetVersionsBySnapshotIdMakesGetCall()
        {
            string id = NewRandomString();

            await AssertGetRequest($"FundingData/FundingDataVersions/{id}",
                id,
                new FDSDatasetVersion(),
                _ => _client.GetDatasetVersionsBySnapshotId(_));
        }

        [TestMethod]
        public async Task GetDatasetVersionsCountByDefinitionIdMakesGetCall()
        {
            string id = NewRandomString();

            await AssertGetRequest($"FundingData/FundingDataVersions/Schema/count/{id}",
                id,
                new FundingDataVersionCount(),
                _ => _client.GetDatasetVersionsCountByDefinitionId(_));
        }

        [TestMethod]
        public async Task GetDatasourceDataMakesGetCall()
        {
            string id = NewRandomString();

            await AssertGetRequest($"FundingData/{id}",
                id,
                new FDSDatasourceDataModel(),
                _ => _client.GetDatasourceDataBySnapshotId(_));
        }
    }
}