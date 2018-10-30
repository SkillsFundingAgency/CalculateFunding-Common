using System.Security.Claims;
using System.Threading.Tasks;
using CalculateFunding.Common.Identity.Authorization.Models;
using CalculateFunding.Common.Identity.Authorization.Repositories;
using CalculateFunding.Common.Utility;
using Microsoft.AspNetCore.Authorization;

namespace CalculateFunding.Common.Identity.Authorization
{
    public class SpecificationPermissionHandler : AuthorizationHandler<SpecificationRequirement, ISpecificationAuthorizationEntity>
    {
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly PermissionOptions _permissionOptions;

        public SpecificationPermissionHandler(IPermissionsRepository permissionsRepository, PermissionOptions permissionOptions)
        {
            Guard.ArgumentNotNull(permissionsRepository, nameof(permissionsRepository));
            Guard.ArgumentNotNull(permissionOptions, nameof(permissionOptions));

            _permissionsRepository = permissionsRepository;
            _permissionOptions = permissionOptions;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SpecificationRequirement requirement, ISpecificationAuthorizationEntity resource)
        {
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

                default:
                    return false;
            }
        }
    }
}
