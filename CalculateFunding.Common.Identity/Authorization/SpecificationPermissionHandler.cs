using System;
using System.Net;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Interfaces;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Users.Models;
using CalculateFunding.Common.Identity.Authorization.Models;
using CalculateFunding.Common.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CalculateFunding.Common.Identity.Authorization
{
    public class SpecificationPermissionHandler : AuthorizationHandler<SpecificationRequirement, string>
    {
        private readonly IUsersApiClient _usersApiClient;
        private readonly PermissionOptions _permissionOptions;
//        private readonly IFeatureToggle _features;

        public SpecificationPermissionHandler(IUsersApiClient usersApiClient, IOptions<PermissionOptions> permissionOptions)
        {
            Guard.ArgumentNotNull(usersApiClient, nameof(usersApiClient));
            Guard.ArgumentNotNull(permissionOptions, nameof(permissionOptions));

            _usersApiClient = usersApiClient;
            _permissionOptions = permissionOptions.Value;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SpecificationRequirement requirement, string specificationId)
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
                    ApiResponse<EffectiveSpecificationPermission> permissionResponse = await _usersApiClient.GetEffectivePermissionsForUser(userId, specificationId);

                    if (permissionResponse == null || permissionResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception($"Error calling the permissions service - {permissionResponse.StatusCode}");
                    }

                    // Check user has permissions for funding stream
                    if (HasPermission(requirement.ActionType, permissionResponse.Content))
                    {
                        context.Succeed(requirement);
                    }
                }
            }
        }

        private bool HasPermission(SpecificationActionTypes requestedPermission, EffectiveSpecificationPermission actualPermissions)
        {
            if (actualPermissions == null)
            {
                return false;
            }

            switch (requestedPermission)
            {
                case SpecificationActionTypes.CanApproveFunding:
                    return actualPermissions.CanApproveFunding;

                case SpecificationActionTypes.CanChooseFunding:
                    return actualPermissions.CanChooseFunding;

                case SpecificationActionTypes.CanEditCalculations:
                    return actualPermissions.CanEditCalculations;

                case SpecificationActionTypes.CanEditSpecification:
                    return actualPermissions.CanEditSpecification;

                case SpecificationActionTypes.CanMapDatasets:
                    return actualPermissions.CanMapDatasets;

                case SpecificationActionTypes.CanReleaseFunding:
                    return actualPermissions.CanReleaseFunding;

                case SpecificationActionTypes.CanRefreshFunding:
                    return actualPermissions.CanRefreshFunding;

                case SpecificationActionTypes.CanCreateQaTests:
                    return actualPermissions.CanCreateQaTests;

                case SpecificationActionTypes.CanEditQaTests:
                    return actualPermissions.CanEditQaTests;

                case SpecificationActionTypes.CanApproveSpecification:
                    return actualPermissions.CanApproveSpecification;

                case SpecificationActionTypes.CanAdministerFundingStream:
                    return actualPermissions.CanAdministerFundingStream;

                case SpecificationActionTypes.CanDeleteSpecification:
                    return actualPermissions.CanDeleteSpecification;

                case SpecificationActionTypes.CanDeleteCalculations:
                    return actualPermissions.CanDeleteCalculations;

                case SpecificationActionTypes.CanDeleteQaTests:
                    return actualPermissions.CanDeleteQaTests;

                default:
                    return false;
            }
        }
    }
}
