using CalculateFunding.Common.ApiClient.Graph.Models;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.Graph
{
    public class GraphApiClient : BaseApiClient, IGraphApiClient
    {
        private const string UrlRoot = "graph";

        public GraphApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider = null)
         : base(httpClientFactory, HttpClientKeys.Calculations, logger, cancellationTokenProvider)
        { }

        public async Task<HttpStatusCode> SaveCalculations(IList<Calculation> calculations)
        {
            Guard.ArgumentNotNull(calculations, nameof(calculations));

            string url = $"{UrlRoot}/calculations";

            return await PostAsync(url, calculations);
        }

        public async Task<HttpStatusCode> DeleteCalculation(string calculationId)
        {
            Guard.IsNullOrWhiteSpace(calculationId, nameof(calculationId));

            string url = $"{UrlRoot}/calculations/{calculationId}";

            return await DeleteAsync(url);
        }
    }
}
