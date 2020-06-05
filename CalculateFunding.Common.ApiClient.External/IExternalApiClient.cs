using System.Collections.Generic;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.External.Models;
using CalculateFunding.Common.ApiClient.Models;

namespace CalculateFunding.Common.ApiClient.External
{
    public interface IExternalApiClient
    {
        Task<ApiResponse<AtomFeed<object>>> GetFundingNotifications(string[] fundingStreamIds = null,
            string[] fundingPeriodIds = null,
            GroupingReason[] groupingReasons = null,
            int? pageSize = null,
            int? pageRef = null);

        Task<ApiResponse<string>> GetFundingById(string id);
        Task<ApiResponse<AtomFeed<object>>> GetProviderFundingVersion(string providerFundingVersion);
        Task<ApiResponse<IEnumerable<dynamic>>> GetFundings(string publishedProviderVersion);
        Task<(bool Ok, string Message)> IsHealthOk();

        Task<ApiResponse<IEnumerable<FundingStream>>> GetFundingStreams();
        Task<ApiResponse<IEnumerable<FundingPeriod>>> GetFundingPeriods(string fundingStreamId);
        Task<ApiResponse<string>> GetFundingTemplateSourceFile(
            string fundingStreamId, string fundingPeriodId, string majorVersion, string minorVersion);
    }
}