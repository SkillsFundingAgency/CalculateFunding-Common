using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.External.Models;
using CalculateFunding.Common.ApiClient.Models;

namespace CalculateFunding.Common.ApiClient.External
{
    public interface IExternalApiClient
    {
        Task<ApiResponse<FundingStream>> GetFundingStreamById(string fundingStreamId);
    }
}
