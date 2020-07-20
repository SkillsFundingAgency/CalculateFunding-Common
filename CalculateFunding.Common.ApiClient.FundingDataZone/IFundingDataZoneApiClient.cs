using System.Collections.Generic;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.FundingDataZone.Models;
using CalculateFunding.Common.ApiClient.Models;

namespace CalculateFunding.Common.ApiClient.FundingDataZone
{
    public interface IFundingDataZoneApiClient
    {
        Task<ApiResponse<object>> GetDataForDatasetVersion(string datasetCode, int versionNumber);
        Task<ApiResponse<IEnumerable<string>>> GetFundingStreamsWithDatasets();
        Task<ApiResponse<IEnumerable<Dataset>>> GetDatasetsAndVersionsForFundingStream(string fundingStreamId);
        Task<ApiResponse<IEnumerable<DatasetMetadata>>> GetDatasetsForFundingStream(string fundingStreamId);
        Task<ApiResponse<IEnumerable<DatasetMetadata>>> GetDatasetVersionsForDataset(
            string fundingStreamId, string datasetCode);
        Task<ApiResponse<IEnumerable<DatasetMetadata>>> GetDatasetMetadataForDataset(
            string fundingStreamId, string datasetCode, int versionNumber);
        Task<ApiResponse<IEnumerable<Provider>>> GetProvidersInSnapshot(
            int providerSnapshotId);
        Task<ApiResponse<IEnumerable<Provider>>> GetProvidersInSnapshot(
            int providerSnapshotId, string providerId);
        Task<ApiResponse<IEnumerable<PaymentOrganisation>>> GetLocalAuthorities(
            int providerSnapshotId);
        Task<ApiResponse<IEnumerable<PaymentOrganisation>>> GetAllOrganisations(
            int providerSnapshotId);
        Task<ApiResponse<IEnumerable<ProviderSnapshot>>> ListFundingStreamsWithProviderSnapshots();
        Task<ApiResponse<IEnumerable<ProviderSnapshot>>> GetProviderSnapshotsForFundingStream(string fundingStreamId);
        Task<ApiResponse<IEnumerable<Provider>>> GetProviderSnapshotMetadata(int providerSnapshotId);
    }
}
