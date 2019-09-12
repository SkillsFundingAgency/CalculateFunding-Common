using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Results.Models;
using CalculateFunding.Common.Models.Search;

namespace CalculateFunding.Common.ApiClient.Results
{
    public interface IResultsApiClient
    {
        Task<ApiResponse<IEnumerable<string>>> GetProviderSpecifications(string providerId);
        Task<ApiResponse<ProviderResult>> GetProviderResults(string providerId, string specificationId);
        Task<ApiResponse<IEnumerable<ProviderSourceDataset>>> GetProviderSourceDataSetsByProviderIdAndSpecificationId(string providerId, string specificationId);
        Task<HttpStatusCode> ReIndexCalculationProviderResults();
        Task<ApiResponse<CalculationProviderResultSearchResults>> SearchCalculationProviderResults(SearchModel search);
        Task<ApiResponse<IEnumerable<string>>> GetScopedProviderIdsBySpecificationId(string specificationId);
        Task<ApiResponse<IEnumerable<FundingCalculationResultsTotals>>> GetFundingCalculationResultsForSpecifications(SpecificationListModel specificationList);
        Task<ApiResponse<IEnumerable<ProviderResult>>> GetProviderResultsBySpecificationId(string specificationId, string top = null);
        Task<ApiResponse<bool>> HasCalculationResults(string calculationId);
        Task<HttpStatusCode> ReIndexCalculationResults();
    }
}