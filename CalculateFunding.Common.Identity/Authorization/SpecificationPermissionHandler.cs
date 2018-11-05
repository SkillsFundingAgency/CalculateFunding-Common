using System.Security.Claims;
using System.Threading.Tasks;
using CalculateFunding.Common.FeatureToggles;
using CalculateFunding.Common.Identity.Authorization.Models;
using CalculateFunding.Common.Identity.Authorization.Repositories;
using CalculateFunding.Common.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace CalculateFunding.Common.Identity.Authorization
{
    public class SpecificationPermissionHandler : AuthorizationHandler<SpecificationRequirement, ISpecificationAuthorizationEntity>
    {
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly PermissionOptions _permissionOptions;
        private readonly IFeatureToggle _features;

        public SpecificationPermissionHandler(IPermissionsRepository permissionsRepository, IOptions<PermissionOptions> permissionOptions, IFeatureToggle features)
        {
            Guard.ArgumentNotNull(permissionsRepository, nameof(permissionsRepository));
            Guard.ArgumentNotNull(permissionOptions, nameof(permissionOptions));
            Guard.ArgumentNotNull(features, nameof(features));

            _permissionsRepository = permissionsRepository;
            _permissionOptions = permissionOptions.Value;
            _features = features;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SpecificationRequirement requirement, ISpecificationAuthorizationEntity resource)
        {
            if (!_features.IsRoleBasedAccessEnabled())
            {
                context.Succeed(requirement);
                return;
            }

            // If user belongs to the admin group then allow them access
            if (context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value.ToLowerInvariant() == _permissionOptions.AdminGroupId.ToString().ToLowerInvariant()))
            {
                context.Succeed(requirement);
            }
            else
            {
                // Get user permissions for funding stream
                if (context.User.HasClaim(c => c.Type == Constants.ObjectIdentifierClaimType))
                {
                    string userId = context.User.FindFirst(Constants.ObjectIdentifierClaimType).Value;
                    EffectiveSpecificationPermission permission = await _permissionsRepository.GetPermissionForUserBySpecificationId(userId, resource.GetSpecificationId());

                    // Check user has permissions for funding stream
                    if (HasPermission(requirement.ActionType, permission))
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

                case SpecificationActionTypes.CanPublishFunding:
                    return actualPermissions.CanPublishFunding;

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

                default:
                    return false;
            }
        }
    }
}
