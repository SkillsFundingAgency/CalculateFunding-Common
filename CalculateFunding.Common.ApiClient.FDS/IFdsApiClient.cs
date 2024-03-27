using CalculateFunding.Common.ApiClient.FDS.Models;
using CalculateFunding.Common.ApiClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.FDS
{
    public interface IFDSApiClient
    {
        Task<ApiResponse<IEnumerable<DatasetDefinitionByFundingStream>>> GetFDSDataSchema(string fundingStream, string fundingPeriod);
        Task<ApiResponse<FDSDatasetDefinition>> GetDatasetDefinition(string definitionId);
        Task<ApiResponse<IEnumerable<FDSDatasetVersion>>> GetDatasetVersionsByDefinitionId(string definitionId);
        Task<ApiResponse<FundingDataVersionCount>> GetDatasetVersionsCountByDefinitionId(string definitionId);
        Task<ApiResponse<FDSDatasourceDataModel>> GetDatasourceDataBySnapshotId(string snapshotId);
        Task<ApiResponse<FDSDatasetVersion>> GetDatasetVersionsBySnapshotId(string snapshotId);
        Task<ApiResponse<IEnumerable<RemovedFieldDefinition>>> GetRemovedFieldsFromFDSDataSchema(string fundingStream, string fundingPeriod, string schemaName);
    }
}
