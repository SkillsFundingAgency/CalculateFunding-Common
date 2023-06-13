using CalculateFunding.Common.ApiClient.FDS.Models;
using CalculateFunding.Common.ApiClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.FDS
{
    public interface IFDSApiClient
    {
        Task<ApiResponse<IEnumerable<DatasetDefinitionByFundingStream>>> GetDatasetForFundingStream(string fundingStream);
        Task<ApiResponse<FDSDatasetDefinition>> GetDatasetDefinition(string definitionId);
        Task<ApiResponse<IEnumerable<FDSDatasetVersion>>> GetDatasetVersionsByDefinitionId(string definitionId);
        Task<ApiResponse<FundingDataVersionCount>> GetDatasetVersionsCountByDefinitionId(string definitionId);
    }
}
