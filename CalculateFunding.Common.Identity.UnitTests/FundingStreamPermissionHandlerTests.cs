using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CalculateFunding.Common.Identity.Authorization;
using CalculateFunding.Common.Identity.Authorization.Models;
using CalculateFunding.Common.Identity.Authorization.Repositories;
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
        private PermissionOptions actualOptions = new PermissionOptions { AdminGroupId = Guid.NewGuid() };

        [TestMethod]
        public async Task WhenUserIsNotKnown_ShouldNotSucceed()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity());
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, fundingStreamIds);

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(permissionsRepository, options);

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
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, fundingStreamIds);

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(permissionsRepository, options);

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
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, fundingStreamIds);

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionsForUser(Arg.Is(userId)).Returns(Enumerable.Empty<FundingStreamPermission>());

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(permissionsRepository, options);

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
                new Claim(ClaimTypes.Role, actualOptions.AdminGroupId.ToString())
            };
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(claims));
            List<string> fundingStreamIds = new List<string> { WellKnownFundingStreamId };
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, fundingStreamIds);

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(permissionsRepository, options);

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
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, fundingStreamIds);

            FundingStreamPermission actualPermission = new FundingStreamPermission
            {
                CanCreateSpecification = true,
                FundingStreamId = WellKnownFundingStreamId
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionsForUser(Arg.Is(userId)).Returns(new List<FundingStreamPermission> { actualPermission });

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(permissionsRepository, options);

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
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, fundingStreamIds);

            FundingStreamPermission actualPermission = new FundingStreamPermission
            {
                CanCreateSpecification = false,
                FundingStreamId = WellKnownFundingStreamId
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionsForUser(Arg.Is(userId)).Returns(new List<FundingStreamPermission> { actualPermission });

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(permissionsRepository, options);

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
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, fundingStreamIds);

            List<FundingStreamPermission> actualPermissions = new List<FundingStreamPermission> {
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = WellKnownFundingStreamId },
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = "fs2" },
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = "fs3" }
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionsForUser(Arg.Is(userId)).Returns(actualPermissions);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(permissionsRepository, options);

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
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, fundingStreamIds);

            List<FundingStreamPermission> actualPermissions = new List<FundingStreamPermission> {
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = WellKnownFundingStreamId },
                new FundingStreamPermission { CanCreateSpecification = false, FundingStreamId = "fs2" },
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = "fs3" }
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionsForUser(Arg.Is(userId)).Returns(actualPermissions);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(permissionsRepository, options);

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
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, fundingStreamIds);

            List<FundingStreamPermission> actualPermissions = new List<FundingStreamPermission> {
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = "fs4" },
                new FundingStreamPermission { CanCreateSpecification = false, FundingStreamId = "fs5" },
                new FundingStreamPermission { CanCreateSpecification = true, FundingStreamId = "fs6" }
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionsForUser(Arg.Is(userId)).Returns(actualPermissions);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            FundingStreamPermissionHandler authHandler = new FundingStreamPermissionHandler(permissionsRepository, options);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeFalse();
        }

        private AuthorizationHandlerContext CreateAuthenticationContext(ClaimsPrincipal principal, IEnumerable<string> resource)
        {
            FundingStreamRequirement requirement = new FundingStreamRequirement(FundingStreamActionTypes.CanCreateSpecification);
            return new AuthorizationHandlerContext(new[] { requirement }, principal, resource);
        }
    }
}
