using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Serilog;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.FundingDataZone;
using CalculateFunding.Common.ApiClient.FundingDataZone.Models;

namespace CalculateFunding.Common.ApiClient.FDZ.UnitTests
{
    [TestClass]
    public class FundingDataZoneApiClientTests : ApiClientTestBase
    {
        private FundingDataZoneApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new FundingDataZoneApiClient(ClientFactory,
                new Mock<ILogger>().Object);
        }

        [TestMethod]
        public async Task GetAllOrganisations()
        {
            int providerSnapshotId = NewRandomInt();

            await AssertGetRequest($"providers/snapshots/{providerSnapshotId}/organisations",
                Enumerable.Empty<PaymentOrganisation>(),
                () => _client.GetAllOrganisations(providerSnapshotId));
        }

        [TestMethod]
        public async Task GetDataForDatasetVersion()
        {
            string datasetCode = NewRandomString();
            int versionNumber = NewRandomInt();

            GivenTheResponse(
                $"datasets/data/{datasetCode}/{versionNumber}", new object(), HttpMethod.Get);

            ApiResponse<object> apiResponse = await _client.GetDataForDatasetVersion(datasetCode, versionNumber);

            apiResponse?.StatusCode
                .Should()
                .Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task GetDatasetMetadataForDataset()
        {
            string fundingStreamId = NewRandomString();
            string datasetCode = NewRandomString();
            int versionNumber = NewRandomInt();

            await AssertGetRequest($"datasets/fundingStreams/{fundingStreamId}/datasets/{datasetCode}/{versionNumber}",
                Enumerable.Empty<DatasetMetadata>(),
                () => _client.GetDatasetMetadataForDataset(fundingStreamId, datasetCode, versionNumber));
        }

        [TestMethod]
        public async Task GetDatasetsAndVersionsForFundingStream()
        {
            string fundingStreamId = NewRandomString();

            await AssertGetRequest($"datasets/fundingStreams/{fundingStreamId}",
                Enumerable.Empty<Dataset>(),
                () => _client.GetDatasetsAndVersionsForFundingStream(fundingStreamId));
        }

        [TestMethod]
        public async Task GetDatasetsForFundingStream()
        {
            string fundingStreamId = NewRandomString();

            await AssertGetRequest($"datasets/fundingStreams/{fundingStreamId}/datasets",
                Enumerable.Empty<DatasetMetadata>(),
                () => _client.GetDatasetsForFundingStream(fundingStreamId));
        }

        [TestMethod]
        public async Task GetDatasetVersionsForDataset()
        {
            string fundingStreamId = NewRandomString();
            string datasetCode = NewRandomString();

            await AssertGetRequest($"datasets/fundingStreams/{fundingStreamId}/datasets/{datasetCode}",
                Enumerable.Empty<DatasetMetadata>(),
                () => _client.GetDatasetVersionsForDataset(fundingStreamId, datasetCode));
        }

        [TestMethod]
        public async Task GetFundingStreamsWithDatasets()
        {
            await AssertGetRequest($"datasets/fundingStreams",
                Enumerable.Empty<string>(),
                () => _client.GetFundingStreamsWithDatasets());
        }

        [TestMethod]
        public async Task GetLocalAuthorities()
        {
            int providerSnapshotId = NewRandomInt();

            await AssertGetRequest($"providers/snapshots/{providerSnapshotId}/localAuthorities",
                Enumerable.Empty<PaymentOrganisation>(),
                () => _client.GetLocalAuthorities(providerSnapshotId));
        }

        [TestMethod]
        public async Task GetProvidersInSnapshot()
        {
            int providerSnapshotId = NewRandomInt();

            await AssertGetRequest($"providers/snapshots/{providerSnapshotId}/providers",
                Enumerable.Empty<Provider>(),
                () => _client.GetProvidersInSnapshot(providerSnapshotId));
        }

        [TestMethod]
        public async Task GetProvidersInSnapshotForGivenProviderId()
        {
            int providerSnapshotId = NewRandomInt();
            string providerId = NewRandomString();

            await AssertGetRequest($"providers/snapshots/{providerSnapshotId}/providers/{providerId}",
                Enumerable.Empty<Provider>(),
                () => _client.GetProvidersInSnapshot(providerSnapshotId, providerId));
        }

        [TestMethod]
        public async Task GetProviderSnapshotMetadata()
        {
            int providerSnapshotId = NewRandomInt();

            await AssertGetRequest($"providers/snapshots/{providerSnapshotId}",
                Enumerable.Empty<Provider>(),
                () => _client.GetProviderSnapshotMetadata(providerSnapshotId));
        }

        [TestMethod]
        public async Task GetProviderSnapshotsForFundingStream()
        {
            string fundingStreamId = NewRandomString();

            await AssertGetRequest($"providers/fundingStreams/{fundingStreamId}/snapshots",
                Enumerable.Empty<ProviderSnapshot>(),
                () => _client.GetProviderSnapshotsForFundingStream(fundingStreamId));
        }

        [TestMethod]
        public async Task ListFundingStreamsWithProviderSnapshots()
        {
            await AssertGetRequest($"providers/fundingStreams",
                Enumerable.Empty<ProviderSnapshot>(),
                () => _client.ListFundingStreamsWithProviderSnapshots());
        }
    }
}
