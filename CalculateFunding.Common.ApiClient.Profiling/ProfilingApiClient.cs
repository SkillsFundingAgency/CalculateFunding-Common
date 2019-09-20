using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Bearer;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Profiling.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Profiling
{
    public class ProfilingApiClient : BearerBaseApiClient, IProfilingApiClient
    {
        public ProfilingApiClient(
            IHttpClientFactory httpClientFactory,
            string clientKey,
            ILogger logger,
            IBearerTokenProvider bearerTokenProvider,
            ICancellationTokenProvider cancellationTokenProvider = null) : base(httpClientFactory, clientKey, logger, bearerTokenProvider, cancellationTokenProvider)
        {
        }

        public async Task<ValidatedApiResponse<ProviderProfilingResponseModel>> GetProviderProfilePeriods(ProviderProfilingRequestModel requestModel)
        {
            Guard.ArgumentNotNull(requestModel, nameof(requestModel));

            return await ValidatedPostAsync<ProviderProfilingResponseModel, ProviderProfilingRequestModel>("profiling", requestModel);
        }

        public async Task<NoValidatedContentApiResponse> SaveProfilingConfig(SetFundingStreamPeriodProfilePatternRequestModel requestModel)
        {
            Guard.ArgumentNotNull(requestModel, nameof(requestModel));

            return await ValidatedPostAsync<SetFundingStreamPeriodProfilePatternRequestModel>("profiling", requestModel);
        }
    }
}
