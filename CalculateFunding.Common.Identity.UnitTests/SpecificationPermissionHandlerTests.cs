using System;
using System.Collections.Generic;
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
        public async Task WhenUserCanEditSpecification_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanEditSpecification, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanEditSpecification = true
            };

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

        [TestMethod]
        public async Task WhenUserCanEditCalculations_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanEditCalculations, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanEditCalculations = true
            };

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

        [TestMethod]
        public async Task WhenUserCanMapDatasets_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanMapDatasets, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanMapDatasets = true
            };

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

        [TestMethod]
        public async Task WhenUserCanChooseFunding_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanChooseFunding, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanChooseFunding = true
            };

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

        [TestMethod]
        public async Task WhenUserCanApproveFunding_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanApproveFunding = true
            };

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

        [TestMethod]
        public async Task WhenUserCanReleaseFunding_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanReleaseFunding, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanReleaseFunding = true
            };

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

        [TestMethod]
        public async Task WhenUserCanRefreshFunding_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanRefreshFunding, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanRefreshFunding = true
            };

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

        [TestMethod]
        public async Task WhenUserCanCreateQaTests_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanCreateQaTests, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanCreateQaTests = true
            };

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

        [TestMethod]
        public async Task WhenUserCanEditQaTests_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanEditQaTests, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanEditQaTests = true
            };

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

        [TestMethod]
        public async Task WhenUserCanApproveSpecification_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveSpecification, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanApproveSpecification = true
            };

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

        [TestMethod]
        public async Task WhenUserCanAdministerFundingStream_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanAdministerFundingStream, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanAdministerFundingStream = true
            };

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
        
        [TestMethod]
        public async Task WhenUserCanRefreshPublishedQaForSpecification_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanRefreshPublishedQa, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanRefreshPublishedQa = true
            };

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
        
        [TestMethod]
        public async Task WhenUserCanUploadDataSourceFilesForSpecification_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            string specification = WellKnownSpecificationId;
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanUploadDataSourceFiles, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanUploadDataSourceFiles = true
            };

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

        private AuthorizationHandlerContext CreateAuthenticationContext(ClaimsPrincipal principal, SpecificationActionTypes permissionRequired, string resource)
        {
            SpecificationRequirement requirement = new SpecificationRequirement(permissionRequired);
            return new AuthorizationHandlerContext(new[] { requirement }, principal, resource);
        }
    }
}
