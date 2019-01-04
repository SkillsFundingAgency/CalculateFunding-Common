﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CalculateFunding.Common.FeatureToggles;
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
    public class SpecificationPermissionHandlerTests
    {
        private const string WellKnownSpecificationId = "abc123";
        private PermissionOptions actualOptions = new PermissionOptions { AdminGroupId = Guid.NewGuid() };

        [TestMethod]
        public async Task WhenUserIsNotKnown_ShouldNotSucceed()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity());
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), WellKnownSpecificationId).Returns(new EffectiveSpecificationPermission());

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            specification.GetSpecificationId().Returns(WellKnownSpecificationId);
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanEditSpecification, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanEditSpecification = true
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(actualPermission);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            specification.GetSpecificationId().Returns(WellKnownSpecificationId);
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanEditCalculations, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanEditCalculations = true
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(actualPermission);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            specification.GetSpecificationId().Returns(WellKnownSpecificationId);
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanMapDatasets, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanMapDatasets = true
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(actualPermission);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            specification.GetSpecificationId().Returns(WellKnownSpecificationId);
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanChooseFunding, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanChooseFunding = true
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(actualPermission);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            specification.GetSpecificationId().Returns(WellKnownSpecificationId);
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanApproveFunding = true
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(actualPermission);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [TestMethod]
        public async Task WhenUserCanPublishFunding_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            specification.GetSpecificationId().Returns(WellKnownSpecificationId);
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanPublishFunding, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanPublishFunding = true
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(actualPermission);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            specification.GetSpecificationId().Returns(WellKnownSpecificationId);
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanRefreshFunding, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanRefreshFunding = true
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(actualPermission);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            specification.GetSpecificationId().Returns(WellKnownSpecificationId);
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanCreateQaTests, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanCreateQaTests = true
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(actualPermission);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            specification.GetSpecificationId().Returns(WellKnownSpecificationId);
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanEditQaTests, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanEditQaTests = true
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(actualPermission);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            specification.GetSpecificationId().Returns(WellKnownSpecificationId);
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveSpecification, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanApproveSpecification = true
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(actualPermission);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

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
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            specification.GetSpecificationId().Returns(WellKnownSpecificationId);
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanAdministerFundingStream, specification);

            EffectiveSpecificationPermission actualPermission = new EffectiveSpecificationPermission
            {
                CanAdministerFundingStream = true
            };

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), Arg.Is(WellKnownSpecificationId)).Returns(actualPermission);

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(true);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [TestMethod]
        public async Task WhenRoleBasedFeatureIsNotEnabled_AndUserIsNotKnown_ShouldSucceed()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity());
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(false);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [TestMethod]
        public async Task WhenRoleBasedFeatureIsNotEnabled_AndUserIsNotKnownToTheSystem_ShouldSucceed()
        {
            // Arrange
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, Guid.NewGuid().ToString()) }));
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(false);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        [TestMethod]
        public async Task WhenRoleBasedFeatureIsNotEnabled_AndUserIsKnown_AndHasNoPermissions_ShouldSucceed()
        {
            // Arrange
            string userId = Guid.NewGuid().ToString();
            ClaimsPrincipal principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(Constants.ObjectIdentifierClaimType, userId) }));
            ISpecificationAuthorizationEntity specification = Substitute.For<ISpecificationAuthorizationEntity>();
            AuthorizationHandlerContext authContext = CreateAuthenticationContext(principal, SpecificationActionTypes.CanApproveFunding, specification);

            IPermissionsRepository permissionsRepository = Substitute.For<IPermissionsRepository>();
            permissionsRepository.GetPermissionForUserBySpecificationId(Arg.Is(userId), WellKnownSpecificationId).Returns(new EffectiveSpecificationPermission());

            IOptions<PermissionOptions> options = Substitute.For<IOptions<PermissionOptions>>();
            options.Value.Returns(actualOptions);

            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(false);

            SpecificationPermissionHandler authHandler = new SpecificationPermissionHandler(permissionsRepository, options, features);

            // Act
            await authHandler.HandleAsync(authContext);

            // Assert
            authContext.HasSucceeded.Should().BeTrue();
        }

        private AuthorizationHandlerContext CreateAuthenticationContext(ClaimsPrincipal principal, SpecificationActionTypes permissionRequired, ISpecificationAuthorizationEntity resource)
        {
            SpecificationRequirement requirement = new SpecificationRequirement(permissionRequired);
            return new AuthorizationHandlerContext(new[] { requirement }, principal, resource);
        }

        private static IFeatureToggle CreateFeatureToggle(bool roleBasedAccessEnabled)
        {
            IFeatureToggle features = Substitute.For<IFeatureToggle>();
            features.IsRoleBasedAccessEnabled().Returns(roleBasedAccessEnabled);
            return features;
        }
    }
}