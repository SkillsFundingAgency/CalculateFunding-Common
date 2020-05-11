using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Bearer
{
    public class BearerBaseApiClient : BaseApiClient
    {
        private readonly IBearerTokenProvider _bearerTokenProvider;
        private readonly ILogger _logger;

        public BearerBaseApiClient(
            IHttpClientFactory httpClientFactory,
            string clientKey,
            ILogger logger,
            IBearerTokenProvider bearerTokenProvider,
            ICancellationTokenProvider cancellationTokenProvider = null) : base(httpClientFactory, clientKey, logger, cancellationTokenProvider)
        {
            Guard.ArgumentNotNull(bearerTokenProvider, nameof(bearerTokenProvider));

            _bearerTokenProvider = bearerTokenProvider;
            _logger = logger;
        }

        public override async Task<HttpClient> GetHttpClient()
        {
            HttpClient httpClient = base.CreateHttpClient();

            string token = await _bearerTokenProvider.GetToken();
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new MissingBearerTokenException("Null or empty token from bearer token provider");
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return httpClient;
        }

        public async Task<(bool Ok, string Message)> IsHealthOk()
        {
            return await _bearerTokenProvider.IsHealthOk();
        }
    }
}
