using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Interfaces;
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
    public class FundingStreamPermissionHandlerTests
    {
        private const string WellKnownFundingStreamId = "fs1";
        private readonly PermissionOptions actualOptions = new PermissionOptions { AdminGroupId = Guid.NewGuid() };

        [TestMethod]
        public async Task WhenUserIsNotKnown_ShouldNotSucceed()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity());
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, fundingStreamIds);

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(usersApiClient, options);

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
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, fundingStreamIds);

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();
            usersApiClient.GetFundingStreamPermissionsForUser(Arg.Any<string>()).Returns(new ApiResponse<IEnumerable<FundingStreamPermission>>(HttpStatusCode.OK, Enumerable.Empty<FundingStreamPermission>()));

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(usersApiClient, options);

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
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, fundingStreamIds);

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();
            usersApiClient.GetFundingStreamPermissionsForUser(Arg.Is(userId)).Returns(new ApiResponse<IEnumerable<FundingStreamPermission>>(HttpStatusCode.OK, Enumerable.Empty<FundingStreamPermission>()));

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(usersApiClient, options);

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
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, fundingStreamIds);

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [TestMethod]
        public async Task WhenUserCanCreateSpecification_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, fundingStreamIds);

            FundingStreamPermission actualPermission = new FundingStreamPermission
            {
                CanCreateSpecification = true,
                FundingStreamId = WellKnownFundingStreamId
            };

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();
            usersApiClient.GetFundingStreamPermissionsForUser(Arg.Is(userId)).Returns(new ApiResponse<IEnumerable<FundingStreamPermission>>(HttpStatusCode.OK, new List<FundingStreamPermission> { actualPermission }));

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [TestMethod]
        public async Task WhenUserCannotCreateSpecification_ShouldNotSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, fundingStreamIds);

            FundingStreamPermission actualPermission = new FundingStreamPermission
            {
                CanCreateSpecification = false,
                FundingStreamId = WellKnownFundingStreamId
            };

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();
            usersApiClient.GetFundingStreamPermissionsForUser(Arg.Is(userId)).Returns(new ApiResponse<IEnumerable<FundingStreamPermission>>(HttpStatusCode.OK, new List<FundingStreamPermission> { actualPermission }));

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeFalse();
        }

        [TestMethod]
        public async Task WhenUserCreatingSpecificationWithMultipleFundingStreams_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId, "fs2", "fs3" };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, fundingStreamIds);

            List<FundingStreamPermission> actualPermissions = new List<FundingStreamPermission> {
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = WellKnownFundingStreamId },
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = "fs2" },
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = "fs3" }
            };

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();
            usersApiClient.GetFundingStreamPermissionsForUser(Arg.Is(userId)).Returns(new ApiResponse<IEnumerable<FundingStreamPermission>>(HttpStatusCode.OK, actualPermissions));

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [TestMethod]
        public async Task WhenUserCreatingSpecificationWithMultipleFundingStreams_AndNotEnoughPermissions_ShouldNotSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId, "fs2", "fs3" };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, fundingStreamIds);

            List<FundingStreamPermission> actualPermissions = new List<FundingStreamPermission> {
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = WellKnownFundingStreamId },
                new FundingStreamPermission { CanCreateSpecification = false, FundingStreamId = "fs2" },
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = "fs3" }
            };

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();
            usersApiClient.GetFundingStreamPermissionsForUser(Arg.Is(userId)).Returns(new ApiResponse<IEnumerable<FundingStreamPermission>>(HttpStatusCode.OK, actualPermissions));

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeFalse();
        }

        [TestMethod]
        public async Task WhenUserCreatingSpecificationWithMultipleFundingStreams_AndDifferentPermissions_ShouldNotSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId, "fs2", "fs3" };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, fundingStreamIds);

            List<FundingStreamPermission> actualPermissions = new List<FundingStreamPermission> {
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = "fs4" },
                new FundingStreamPermission { CanCreateSpecification = false, FundingStreamId = "fs5" },
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = "fs6" }
            };

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();
            usersApiClient.GetFundingStreamPermissionsForUser(Arg.Is(userId)).Returns(new ApiResponse<IEnumerable<FundingStreamPermission>>(HttpStatusCode.OK, actualPermissions));

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeFalse();
        }

        [TestMethod]
        public async Task WhenUserApproveCalculationForFundingStream_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanApproveCalculations, fundingStreamIds);

            List<FundingStreamPermission> actualPermissions = new List<FundingStreamPermission> {
                new FundingStreamPermission { CanApproveCalculations = true, FundingStreamId = WellKnownFundingStreamId },
                new FundingStreamPermission { CanApproveCalculations = true, FundingStreamId = "fs2" },
                new FundingStreamPermission { CanApproveCalculations = false, FundingStreamId = "fs3" }
            };

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();
            usersApiClient.GetFundingStreamPermissionsForUser(Arg.Is(userId)).Returns(new ApiResponse<IEnumerable<FundingStreamPermission>>(HttpStatusCode.OK, actualPermissions));

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [TestMethod]
        public async Task WhenUserApproveCalculationForDifferentFundingStreams_AndDifferentPermissions_ShouldNotSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId, "fs2", "fs3" };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanApproveCalculations, fundingStreamIds);

            List<FundingStreamPermission> actualPermissions = new List<FundingStreamPermission> {
                new FundingStreamPermission { CanApproveCalculations = true, FundingStreamId = "fs4" },
                new FundingStreamPermission { CanApproveCalculations = false, FundingStreamId = "fs5" },
                new FundingStreamPermission { CanApproveCalculations = true, FundingStreamId = "fs6" }
            };

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();
            usersApiClient.GetFundingStreamPermissionsForUser(Arg.Is(userId)).Returns(new ApiResponse<IEnumerable<FundingStreamPermission>>(HttpStatusCode.OK, actualPermissions));

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(usersApiClient, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeFalse();
        }

        [TestMethod]
        [Ignore]
        public async Task WhenRoleBasedFeatureIsNotEnabled_AndUserIsNotKnown_ShouldSucceed()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity());
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, FundingStreamActionTypes.CanCreateSpecification, fundingStreamIds);

            IUsersApiClient usersApiClient = Substitute.For<IUsersApiClient>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

//            IFeatureToggle features = CreateFeatureToggle(false);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(usersApiClient, options);

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
