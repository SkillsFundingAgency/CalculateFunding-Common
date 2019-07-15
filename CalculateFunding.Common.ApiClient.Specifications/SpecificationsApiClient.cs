using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Specifications.Models;
using CalculateFunding.Common.Interfaces;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Specifications
{
    public class SpecificationsApiClient : BaseApiClient, ISpecificationsApiClient
    {
        private const string UrlRoot = "specs";

        public SpecificationsApiClient(IHttpClientFactory httpClientFactory,
            ILogger logger,
            ICancellationTokenProvider cancellationTokenProvider = null)
            : base(httpClientFactory, HttpClientKeys.Specifications, logger, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<SpecificationSummary>> GetSpecificationSummaryById(string specificationId)
        {
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<SpecificationSummary>(
                $"{UrlRoot}/specification-summary-by-id?specificationId={specificationId}");
        }

        public async Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsSelectedForFundingByPeriod(
            string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            return await GetAsync<IEnumerable<SpecificationSummary>>(
                $"{UrlRoot}/specifications-selected-for-funding-by-period?fundingPeriodId={fundingPeriodId}");
        }
    }
}