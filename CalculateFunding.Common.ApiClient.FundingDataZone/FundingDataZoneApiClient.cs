using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.FundingDataZone.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.FundingDataZone
{
    public class FundingDataZoneApiClient : BaseApiClient, IFundingDataZoneApiClient
    {
        public FundingDataZoneApiClient(
            IHttpClientFactory httpClientFactory, 
            ILogger logger, 
            ICancellationTokenProvider cancellationTokenProvider = null)
            : base(httpClientFactory, HttpClientKeys.FDZ, logger, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<IEnumerable<PaymentOrganisation>>> GetAllOrganisations(int providerSnapshotId)
        {
            return await GetAsync<IEnumerable<PaymentOrganisation>>(
                $"providers/snapshots/{providerSnapshotId}/organisations");
        }

        public async Task<ApiResponse<object>> GetDataForDatasetVersion(string datasetCode, int versionNumber)
        {
            Guard.IsNullOrWhiteSpace(datasetCode, nameof(datasetCode));

            return await GetAsync<object>($"datasets/data/{datasetCode}/{versionNumber}");
        }

        public async Task<ApiResponse<IEnumerable<DatasetMetadata>>> GetDatasetMetadataForDataset(
            string fundingStreamId, string datasetCode, int versionNumber)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(datasetCode, nameof(datasetCode));

            return await GetAsync<IEnumerable<DatasetMetadata>>(
                $"datasets/fundingStreams/{fundingStreamId}/datasets/{datasetCode}/{versionNumber}");
        }

        public async Task<ApiResponse<IEnumerable<Dataset>>> GetDatasetsAndVersionsForFundingStream(string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<IEnumerable<Dataset>>($"datasets/fundingStreams/{fundingStreamId}");
        }

        public async Task<ApiResponse<IEnumerable<DatasetMetadata>>> GetDatasetsForFundingStream(string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<IEnumerable<DatasetMetadata>>($"datasets/fundingStreams/{fundingStreamId}/datasets");
        }

        public async Task<ApiResponse<IEnumerable<DatasetMetadata>>> GetDatasetVersionsForDataset(
            string fundingStreamId, string datasetCode)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(datasetCode, nameof(datasetCode));

            return await GetAsync<IEnumerable<DatasetMetadata>>(
                $"datasets/fundingStreams/{fundingStreamId}/datasets/{datasetCode}");
        }

        public async Task<ApiResponse<IEnumerable<string>>> GetFundingStreamsWithDatasets()
        {
            return await GetAsync<IEnumerable<string>>($"datasets/fundingStreams");
        }

        public async Task<ApiResponse<IEnumerable<PaymentOrganisation>>> GetLocalAuthorities(int providerSnapshotId)
        {
            return await GetAsync<IEnumerable<PaymentOrganisation>>(
                $"providers/snapshots/{providerSnapshotId}/localAuthorities");
        }

        public async Task<ApiResponse<IEnumerable<Provider>>> GetProvidersInSnapshot(int providerSnapshotId)
        {
            return await GetAsync<IEnumerable<Provider>>($"providers/snapshots/{providerSnapshotId}/providers");
        }

        public async Task<ApiResponse<Provider>> GetProviderInSnapshot(
            int providerSnapshotId, string providerId)
        {
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            return await GetAsync<Provider>(
                $"providers/snapshots/{providerSnapshotId}/providers/{providerId}");
        }

        public async Task<ApiResponse<IEnumerable<Provider>>> GetProviderSnapshotMetadata(int providerSnapshotId)
        {
            return await GetAsync<IEnumerable<Provider>>($"providers/snapshots/{providerSnapshotId}");
        }

        public async Task<ApiResponse<IEnumerable<ProviderSnapshot>>> GetProviderSnapshotsForFundingStream(
            string fundingStreamId ,string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<IEnumerable<ProviderSnapshot>>($"providers/fundingStreams/{fundingStreamId}/{fundingPeriodId}/snapshots");
        }

        public async Task<ApiResponse<IEnumerable<string>>> ListFundingStreamsWithProviderSnapshots()
        {
            return await GetAsync<IEnumerable<string>>($"providers/fundingStreams");
        }

        public async Task<ApiResponse<IEnumerable<ProviderSnapshot>>> GetLatestProviderSnapshotsForAllFundingStreams()
        {
            return await GetAsync<IEnumerable<ProviderSnapshot>>($"providers/fundingStreams/snapshots/latest");
        }

        public async Task<ApiResponse<IEnumerable<ProviderSnapshot>>> GetLatestProviderSnapshotsForAllFundingStreamsWithFundingPeriod()
        {
            return await GetAsync<IEnumerable<ProviderSnapshot>>($"providers/fundingStreams/FundingPeriod/snapshots/latest");
        }
    }
}
