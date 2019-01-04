﻿using System.Collections.Generic;
using System.Linq;
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
    public class FundingStreamPermissionHandler : AuthorizationHandler<FundingStreamRequirement, IEnumerable<string>>
    {
        private readonly IPermissionsRepository _permissionsRepository;
        private readonly PermissionOptions _permissionOptions;
        private readonly IFeatureToggle _features;

        public FundingStreamPermissionHandler(IPermissionsRepository permissionsRepository, IOptions<PermissionOptions> permissionOptions, IFeatureToggle features)
        {
            Guard.ArgumentNotNull(permissionsRepository, nameof(permissionsRepository));
            Guard.ArgumentNotNull(permissionOptions, nameof(permissionOptions));
            Guard.ArgumentNotNull(features, nameof(features));

            _permissionsRepository = permissionsRepository;
            _permissionOptions = permissionOptions.Value;
            _features = features;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, FundingStreamRequirement requirement, IEnumerable<string> resource)
        {
            if (!_features.IsRoleBasedAccessEnabled())
            {
                context.Succeed(requirement);
                return;
            }

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
                    IEnumerable<FundingStreamPermission> permissions = await _permissionsRepository.GetPermissionsForUser(userId);

                    // Check user has permissions for funding stream
                    if (HasPermissionToAllFundingStreams(resource, requirement.ActionType, permissions))
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

            if (requestedPermission == FundingStreamActionTypes.CanCreateSpecification)
            {
                foreach (string item in fundingStreamIds)
                {
                    FundingStreamPermission foundPermission = actualPermissions.FirstOrDefault(p => p.FundingStreamId == item && p.CanCreateSpecification);

                    if (foundPermission == null)
                    {
                        // A required permission is missing so can't succeed
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}