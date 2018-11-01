using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CalculateFunding.Common.Identity.Authorization;
using CalculateFunding.Common.Identity.Authorization.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalculateFunding.Common.Identity.UnitTests
{
    [TestClass]
    public class AlwaysAllowedForFundingStreamPermissionHandlerTests
    {
        [TestMethod]
        public async Task WhenUserIsNotKnown_ShouldSucceed()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity());
            IEnumerable<string> fundingStreamIds = new List<string>();
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, fundingStreamIds);

            AlwaysAllowedForFundingStreamPermissionHandler authHandler = new AlwaysAllowedForFundingStreamPermissionHandler();

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
            IEnumerable<string> fundingStreamIds = new List<string>();
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, fundingStreamIds);

            AlwaysAllowedForFundingStreamPermissionHandler authHandler = new AlwaysAllowedForFundingStreamPermissionHandler();

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
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, null);

            AlwaysAllowedForFundingStreamPermissionHandler authHandler = new AlwaysAllowedForFundingStreamPermissionHandler();

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        private AuthorizationHandlerContext CreateAuthenticationContext(ClaimsPrincipal principal, FundingStreamActionTypes permissionRequired, IEnumerable<string> resource)
        {
            FundingStreamRequirement requirement = new FundingStreamRequirement(permissionRequired);
            return new AuthorizationHandlerContext(new[] { requirement }, principal, resource);
        }
    }
}
