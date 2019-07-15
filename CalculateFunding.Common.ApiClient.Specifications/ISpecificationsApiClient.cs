using System.Collections.Generic;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Specifications.Models;

namespace CalculateFunding.Common.ApiClient.Specifications
{
    public interface ISpecificationsApiClient
    {
        Task<ApiResponse<SpecificationSummary>> GetSpecificationSummaryById(string specificationId);

        Task<ApiResponse<IEnumerable<SpecificationSummary>>> GetSpecificationsSelectedForFundingByPeriod(
            string fundingPeriodId);
    }
}