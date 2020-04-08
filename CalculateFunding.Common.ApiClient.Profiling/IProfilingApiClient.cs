using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Profiling.Models;

namespace CalculateFunding.Common.ApiClient.Profiling
{
    public interface IProfilingApiClient
    {
        Task<ValidatedApiResponse<ProviderProfilingResponseModel>> GetProviderProfilePeriods(ProviderProfilingRequestModel requestModel);
        Task<HttpStatusCode> CreateProfilePattern(CreateProfilePatternRequest request);
        Task<HttpStatusCode> EditProfilePattern(EditProfilePatternRequest request);
        Task<HttpStatusCode> DeleteProfilePattern(string id);
        Task<ApiResponse<FundingStreamPeriodProfilePattern>> GetProfilePattern(string id);

        Task<ApiResponse<IEnumerable<FundingStreamPeriodProfilePattern>>> GetProfilePatternsForFundingStreamAndFundingPeriod(string fundingStreamId,
            string fundingPeriodId);
    }
}
