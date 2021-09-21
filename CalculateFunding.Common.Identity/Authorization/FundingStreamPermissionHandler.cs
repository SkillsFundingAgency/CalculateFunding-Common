using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Users;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Users.Models;
using CalculateFunding.Common.Identity.Authorization.Models;
using CalculateFunding.Common.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CalculateFunding.Common.Identity.Authorization
{
    public class FundingStreamPermissionHandler : AuthorizationHandler<FundingStreamRequirement, IEnumerable<string>>
    {
        private readonly IUsersApiClient _usersApiClient;
        private readonly PermissionOptions _permissionOptions;

        public FundingStreamPermissionHandler(IUsersApiClient usersApiClient, IOptions<PermissionOptions> permissionOptions)
        {
            Guard.ArgumentNotNull(usersApiClient, nameof(usersApiClient));
            Guard.ArgumentNotNull(permissionOptions, nameof(permissionOptions));

            _usersApiClient = usersApiClient;
            _permissionOptions = permissionOptions.Value;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, FundingStreamRequirement requirement, IEnumerable<string> resource)
        {
            // If user belongs to the admin group then allow them access
            if (context.User.HasClaim(c => c.Type == Constants.GroupsClaimType && c.Value.ToLowerInvariant() == _permissionOptions.AdminGroupId.ToString().ToLowerInvariant()))
            {
                context.Succeed(requirement);
            }
            else
            {
                // Get user permissions for funding stream
                if (context.User.HasClaim(c => c.Type == Constants.ObjectIdentifierClaimType))
                {
                    string userId = context.User.FindFirst(Constants.ObjectIdentifierClaimType).Value;
                    ApiResponse<IEnumerable<FundingStreamPermission>> permissionsResponse = await _usersApiClient.GetFundingStreamPermissionsForUser(userId);

                    if (permissionsResponse == null || permissionsResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception($"Error calling the permissions service - {permissionsResponse.StatusCode}");
                    }

                    // Check user has permissions for funding stream
                    if (HasPermissionToAllFundingStreams(resource, requirement.ActionType, permissionsResponse.Content))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }

        private bool HasPermissionToAllFundingStreams(IEnumerable<string> fundingStreamIds, FundingStreamActionTypes requestedPermission, IEnumerable<FundingStreamPermission> actualPermissions)
        {
            if (actualPermissions == null || actualPermissions.Count() == 0)
            {
                // No permissions to check against so can't have permission for the action
                return false;
            }

            return requestedPermission switch
            {
                FundingStreamActionTypes.CanCreateSpecification => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanCreateSpecification)),
                FundingStreamActionTypes.CanChooseFunding => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanChooseFunding)),
                FundingStreamActionTypes.CanCreateTemplates => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanCreateTemplates)),
                FundingStreamActionTypes.CanEditTemplates => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanEditTemplates)),
                FundingStreamActionTypes.CanApproveTemplates => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanApproveTemplates)),
                FundingStreamActionTypes.CanCreateProfilePattern => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanCreateProfilePattern)),
                FundingStreamActionTypes.CanEditProfilePattern => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanEditProfilePattern)),
                FundingStreamActionTypes.CanAssignProfilePattern => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanAssignProfilePattern)),
                FundingStreamActionTypes.CanApplyCustomProfilePattern => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanApplyCustomProfilePattern)),
                FundingStreamActionTypes.CanApproveCalculations => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanApproveCalculations)),
                FundingStreamActionTypes.CanApproveAnyCalculations => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanApproveAnyCalculations)),
                FundingStreamActionTypes.CanRefreshPublishedQa => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanRefreshPublishedQa)),
                FundingStreamActionTypes.CanUploadDataSourceFiles => fundingStreamIds.All(fs => actualPermissions.Any(p => p.FundingStreamId == fs && p.CanUploadDataSourceFiles)),
                _ => false,
            };
        }
    }
}
