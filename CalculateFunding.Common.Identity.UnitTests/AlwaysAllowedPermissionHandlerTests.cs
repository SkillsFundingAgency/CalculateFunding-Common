﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CalculateFunding.Common.Identity.Authorization;
using CalculateFunding.Common.Identity.Authorization.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CalculateFunding.Common.Identity.UnitTests
{
    [TestClass]
    public class AlwaysAllowedPermissionHandlerTests
    {
        private const string WellKnownFundingStreamId = "abc123";

        [TestMethod]
        public async Task WhenUserIsNotKnown_ShouldSucceed()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity());
            string spec = null;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, spec);

            AlwaysAllowedPermissionHandler authHandler = new AlwaysAllowedPermissionHandler();

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [TestMethod]
        public async Task WhenUserIsKnown_AndHasNoPermissions_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string spec = null;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, spec);

            AlwaysAllowedPermissionHandler authHandler = new AlwaysAllowedPermissionHandler();

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [TestMethod]
        public async Task WhenPassNullResource_AndHasNoPermissions_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, null);

            AlwaysAllowedPermissionHandler authHandler = new AlwaysAllowedPermissionHandler();

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        private AuthorizationHandlerContext CreateAuthenticationContext(ClaimsPrincipal principal, SpecificationActionTypes permissionRequired, string resource)
        {
            SpecificationRequirement requirement = new SpecificationRequirement(permissionRequired);
            return new AuthorizationHandlerContext(new[] { requirement }, principal, resource);
        }
    }
}
