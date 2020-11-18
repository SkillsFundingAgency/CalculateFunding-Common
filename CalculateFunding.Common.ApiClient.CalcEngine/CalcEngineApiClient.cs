using CalculateFunding.Common.ApiClient.CalcEngine.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;
using System.Net.Http;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.CalcEngine
{
    public class CalcEngineApiClient : BaseApiClient, ICalcEngineApiClient
    {
        public CalcEngineApiClient(
            IHttpClientFactory httpClientFactory, 
            ILogger logger, 
            ICancellationTokenProvider cancellationTokenProvider = null)
            : base(httpClientFactory, HttpClientKeys.CalcEngine, logger, cancellationTokenProvider){ }

        public async Task<ApiResponse<ProviderResult>> PreviewCalculationResults(
            string specificationId, 
            string providerId,
            byte[] assemblyContent)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));
            Guard.IsNullOrWhiteSpace(providerId, nameof(providerId));

            string url = $"calculations-results/{specificationId}/{providerId}/preview";

            return await PostAsync<ProviderResult, byte[]>(url, assemblyContent);
        }
    }
}
