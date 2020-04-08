using System.Collections.Generic;
using System.Net;
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

        public async Task<HttpStatusCode> CreateProfilePattern(CreateProfilePatternRequest request)
        {
            Guard.ArgumentNotNull(request, nameof(request));

            return await PostAsync("profiling/patterns", request);
        } 
        
        public async Task<HttpStatusCode> EditProfilePattern(EditProfilePatternRequest request)
        {
            Guard.ArgumentNotNull(request, nameof(request));

            return await PutAsync("profiling/patterns", request);
        }

        public async Task<HttpStatusCode> DeleteProfilePattern(string id)
        {
            Guard.ArgumentNotNull(id, nameof(id));

            return await DeleteAsync($"profiling/patterns/{id}");
        }

        public async Task<ApiResponse<FundingStreamPeriodProfilePattern>> GetProfilePattern(string id)
        {
            Guard.IsNullOrWhiteSpace(id, nameof(id));

            return await GetAsync<FundingStreamPeriodProfilePattern>($"profiling/patterns/{id}");
        }
        
        public async Task<ApiResponse<IEnumerable<FundingStreamPeriodProfilePattern>>> GetProfilePatternsForFundingStreamAndFundingPeriod(string fundingStreamId,
            string fundingPeriodId)
        {
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.IsNullOrWhiteSpace(fundingPeriodId, nameof(fundingPeriodId));

            return await GetAsync<IEnumerable<FundingStreamPeriodProfilePattern>>($"profiling/patterns/fundingStreams/{fundingStreamId}/fundingPeriods/{fundingPeriodId}");
        }
    }
}
