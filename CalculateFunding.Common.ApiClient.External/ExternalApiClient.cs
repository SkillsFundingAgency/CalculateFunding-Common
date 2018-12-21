using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.External.Models;
using CalculateFunding.Common.ApiClient.Interfaces;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.External
{
    public class ExternalApiClient : BaseApiClient, IExternalApiClient
    {
        public ExternalApiClient(
            IHttpClientFactory httpClientFactory,
            string clientKey,
            ILogger logger,
            ICancellationTokenProvider cancellationTokenProvider = null) : base(httpClientFactory, clientKey, logger, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<FundingStream>> GetFundingStreamById(string fundingStreamId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));

            return await GetAsync<FundingStream>($"v2.0/funding-streams/{fundingStreamId}");
        }
    }
}
