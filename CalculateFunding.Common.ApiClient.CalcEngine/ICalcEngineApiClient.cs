using CalculateFunding.Common.ApiClient.CalcEngine.Models;
using CalculateFunding.Common.ApiClient.Models;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.CalcEngine
{
    public interface ICalcEngineApiClient
    {
        Task<ApiResponse<ProviderResult>> PreviewCalculationResults(
            string specificationId, 
            string providerId,
            PreviewCalculationRequest previewCalculationRequest);
    }
}
