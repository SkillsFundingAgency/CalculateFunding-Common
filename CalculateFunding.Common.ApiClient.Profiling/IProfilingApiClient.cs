using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Profiling.Models;

namespace CalculateFunding.Common.ApiClient.Profiling
{
    public interface IProfilingApiClient
    {
        Task<ValidatedApiResponse<ProviderProfilingResponseModel>> GetProviderProfilePeriods(ProviderProfilingRequestModel requestModel);
    }
}
