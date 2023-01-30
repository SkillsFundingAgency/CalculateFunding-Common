using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Users;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.ApiClient.Users.Models;
using CalculateFunding.Common.Identity.Authorization;
using CalculateFunding.Common.Identity.Authorization.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CalculateFunding.Common.Identity.UnitTests
{
    [TestClass]
    public class SpecificationPermissionHandlerTests
    {
        private const string WellKnownSpecificationId = "abc123";
        private PermissionOptions actualOptions = new PermissionOptions { AdminGroupId = Guid.NewGuid() };

        [TestMethod]
        public async Task WhenUserIsNotKnown_ShouldNotSucceed()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity());
            string specification = null;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeFalse();
        }

        [TestMethod]
        public async Task WhenUserIsNotKnownToTheSystem_ShouldNotSucceed()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, Guid.NewGuid().ToString()) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();
            usersApiClient.GetEffectivePermissionsForUser(Arg.Any<string>(), Arg.Is(WellKnownSpecificationId)).Returns(new ApiResponse<EffectiveSpecificationPermission>(HttpStatusCode.OK, new EffectiveSpecificationPermission()));
            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeFalse();
        }

        [TestMethod]
        public async Task WhenUserIsKnown_AndHasNoPermissions_ShouldNotSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();
            usersApiClient.GetEffectivePermissionsForUser(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(new ApiResponse<EffectiveSpecificationPermission>(HttpStatusCode.OK, new EffectiveSpecificationPermission()));

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeFalse();
        }

        [TestMethod]
        public async Task WhenUserIsAdmin_ShouldSucceed()
        {
            // Arrange
            List<Claim> claims = new List<Claim>
            {
                new Claim(Constants.ObjectIdentifierClaimType, Guid.NewGuid().ToString()),
                new Claim(Constants.GroupsClaimType, actualOptions.AdminGroupId.ToString())
            };
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [TestMethod]
        [DynamicData(nameof(EffectiveSpecificationPermissionExamples), DynamicDataSourceType.Method)]
        public async Task WhenUserWithSpecificPermission_ShouldSucceed(
            EffectiveSpecificationPermission actualPermission,
            SpecificationActionTypes specificationActionTypes)
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, specificationActionTypes, specification);

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();
            usersApiClient.GetEffectivePermissionsForUser(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(new ApiResponse<EffectiveSpecificationPermission>(HttpStatusCode.OK, actualPermission));

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        private static IEnumerable<object[]> EffectiveSpecificationPermissionExamples()
        {
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanAdministerFundingStream = true },
                SpecificationActionTypes.CanAdministerFundingStream
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanEditSpecification = true },
                SpecificationActionTypes.CanEditSpecification
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanApproveSpecification = true },
                SpecificationActionTypes.CanApproveSpecification
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanEditCalculations = true },
                SpecificationActionTypes.CanEditCalculations
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanApproveCalculations = true },
                SpecificationActionTypes.CanApproveCalculations
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanApproveAnyCalculations = true },
                SpecificationActionTypes.CanApproveAnyCalculations
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanMapDatasets = true },
                SpecificationActionTypes.CanMapDatasets
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanChooseFunding = true },
                SpecificationActionTypes.CanChooseFunding
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanRefreshFunding = true },
                SpecificationActionTypes.CanRefreshFunding
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanApproveFunding = true },
                SpecificationActionTypes.CanApproveFunding
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanReleaseFunding = true },
                SpecificationActionTypes.CanReleaseFunding
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanReleaseFundingForStatement = true },
                SpecificationActionTypes.CanReleaseFundingForStatement
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanReleaseFundingForPaymentOrContract = true },
                SpecificationActionTypes.CanReleaseFundingForPaymentOrContract
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanApproveAllCalculations = true },
                SpecificationActionTypes.CanApproveAllCalculations
            };
            yield return new object[]
            {
                new EffectiveSpecificationPermission { CanRefreshPublishedQa = true },
                SpecificationActionTypes.CanRefreshPublishedQa
            };
        }

        private AuthorizationHandlerContext CreateAuthenticationContext(ClaimsPrincipal principal, SpecificationActionTypes permissionRequired, string resource)
        {
            SpecificationRequirement requirement = new SpecificationRequirement(permissionRequired);
            return new AuthorizationHandlerContext(new[] { requirement }, principal, resource);
        }
    }
}
