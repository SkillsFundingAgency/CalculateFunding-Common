using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Interfaces;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Users.Models;
using CalculateFunding.Common.Utility;
using Serilog;

namespace CalculateFunding.Common.ApiClient.Users
{
    public class UsersApiClient : BaseApiClient, IUsersApiClient
    {
        public UsersApiClient(IHttpClientFactory httpClientFactory, ILogger logger, ICancellationTokenProvider cancellationTokenProvider)
           : base(httpClientFactory, HttpClientKeys.Users, logger, cancellationTokenProvider)
        {
        }

        public async Task<ApiResponse<User>> GetUserByUserId(string userId)
        {
            Guard.IsNullOrWhiteSpace(userId, nameof(userId));

            return await GetAsync<User>($"get-user-by-userid?userId={userId}");
        }

        public async Task<ValidatedApiResponse<User>> ConfirmSkills(string userId, UserConfirmModel userConfirmModel)
        {
            Guard.IsNullOrWhiteSpace(userId, nameof(userId));

            return await ValidatedPostAsync<User, UserConfirmModel>($"confirm-skills?userId={userId}", userConfirmModel);
        }

        public async Task<ApiResponse<IEnumerable<FundingStreamPermission>>> GetFundingStreamPermissionsForUser(string userId)
        {
            Guard.IsNullOrWhiteSpace(userId, nameof(userId));

            return await GetAsync<IEnumerable<FundingStreamPermission>>($"{userId}/permissions");
        }

        public async Task<ApiResponse<EffectiveSpecificationPermission>> GetEffectivePermissionsForUser(string userId, string specificationId)
        {
            Guard.IsNullOrWhiteSpace(userId, nameof(userId));
            Guard.IsNullOrWhiteSpace(specificationId, nameof(specificationId));

            return await GetAsync<EffectiveSpecificationPermission>($"{userId}/effectivepermissions/{specificationId}");
        }

        public async Task<ValidatedApiResponse<FundingStreamPermission>> UpdateFundingStreamPermission(string userId, string fundingStreamId, FundingStreamPermissionUpdateModel permissions)
        {
            Guard.IsNullOrWhiteSpace(userId, nameof(userId));
            Guard.IsNullOrWhiteSpace(fundingStreamId, nameof(fundingStreamId));
            Guard.ArgumentNotNull(permissions, nameof(permissions));

            return await ValidatedPutAsync<FundingStreamPermission, FundingStreamPermissionUpdateModel>($"api/users/{userId}/permissions/{fundingStreamId}", permissions);
        }
    }
}
