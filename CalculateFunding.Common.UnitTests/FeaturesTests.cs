using CalculateFunding.Common.FeatureToggles;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CalculateFunding.Common.UnitTests
{
    [TestClass]
    public partial class FeaturesTests
    {
        [TestMethod]
        public void IsProviderProfilingServiceEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerProfilingServiceDisabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderProfilingServiceDisabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProviderProfilingServiceEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerProfilingServiceDisabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderProfilingServiceDisabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProviderProfilingServiceEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerProfilingServiceDisabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderProfilingServiceDisabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProviderProfilingServiceEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerProfilingServiceDisabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderProfilingServiceDisabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProviderProfilingServiceEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerProfilingServiceDisabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderProfilingServiceDisabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsPublishButtonEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("publishButtonEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsPublishButtonEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsPublishButtonEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("publishButtonEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsPublishButtonEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsPublishButtonEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("publishButtonEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsPublishButtonEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsPublishButtonEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("publishButtonEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsPublishButtonEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsPublishButtonEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("publishButtonEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsPublishButtonEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsCalculationTimeoutEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("calculationTimeoutEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCalculationTimeoutEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCalculationTimeoutEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("calculationTimeoutEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCalculationTimeoutEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCalculationTimeoutEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("calculationTimeoutEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCalculationTimeoutEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCalculationTimeoutEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("calculationTimeoutEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCalculationTimeoutEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCalculationTimeoutEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("calculationTimeoutEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCalculationTimeoutEnabled();

            // Assert
            result.Should().BeTrue();
        }

		[TestMethod]
		public void IsPublishAndApproveFilterEnabled_WhenConfigNotPresent_ReturnsFalse()
		{
			// Arrange
			IConfigurationSection config = Substitute.For<IConfigurationSection>();
			config[Arg.Is("publishAndApprovePageFiltersEnabled")].Returns((string)null);

			IFeatureToggle features = new Features(config);

			// Act
			bool result = features.IsPublishAndApprovePageFiltersEnabled();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsPublishAndApproveFilterEnabled_WhenConfigEmptyString_ReturnsFalse()
		{
			// Arrange
			IConfigurationSection config = Substitute.For<IConfigurationSection>();
			config[Arg.Is("publishAndApprovePageFiltersEnabled")].Returns(string.Empty);

			IFeatureToggle features = new Features(config);

			// Act
			bool result = features.IsPublishAndApprovePageFiltersEnabled();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsPublishAndApproveFilterEnabled_WhenConfigNotBoolean_ReturnsFalse()
		{
			// Arrange
			IConfigurationSection config = Substitute.For<IConfigurationSection>();
			config[Arg.Is("publishAndApprovePageFiltersEnabled")].Returns("not a bool");

			IFeatureToggle features = new Features(config);

			// Act
			bool result = features.IsPublishAndApprovePageFiltersEnabled();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsPublishAndApproveFilterEnabled_WhenConfigFalse_ReturnsFalse()
		{
			// Arrange
			IConfigurationSection config = Substitute.For<IConfigurationSection>();
			config[Arg.Is("publishAndApprovePageFiltersEnabled")].Returns("false");

			IFeatureToggle features = new Features(config);

			// Act
			bool result = features.IsPublishAndApprovePageFiltersEnabled();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsPublishAndApproveFilterEnabled_WhenConfigTrue_ReturnsTrue()
		{
			// Arrange
			IConfigurationSection config = Substitute.For<IConfigurationSection>();
			config[Arg.Is("publishAndApprovePageFiltersEnabled")].Returns("true");

			IFeatureToggle features = new Features(config);

			// Act
			bool result = features.IsPublishAndApprovePageFiltersEnabled();

			// Assert
			result.Should().BeTrue();
		}

        [TestMethod]
        public void IsRoleBasedAccessEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("roleBasedAccessEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsRoleBasedAccessEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsRoleBasedAccessEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("roleBasedAccessEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsRoleBasedAccessEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsRoleBasedAccessEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("roleBasedAccessEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsRoleBasedAccessEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsRoleBasedAccessEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("roleBasedAccessEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsRoleBasedAccessEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsRoleBasedAccessEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("roleBasedAccessEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsRoleBasedAccessEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsNotificationsEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("notificationsEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNotificationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNotificationsEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("notificationsEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNotificationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNotificationsEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("notificationsEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNotificationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNotificationsEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("notificationsEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNotificationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNotificationsEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("notificationsEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNotificationsEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsNewEditCalculationPageEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newEditCalculationPageEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewEditCalculationPageEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNewEditCalculationPageEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newEditCalculationPageEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewEditCalculationPageEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNewEditCalculationPageEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newEditCalculationPageEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewEditCalculationPageEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNewEditCalculationPageEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newEditCalculationPageEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewEditCalculationPageEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNewEditCalculationPageEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newEditCalculationPageEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewEditCalculationPageEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsNewManageDataSourcesPageEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newManageDataSourcesPageEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewManageDataSourcesPageEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNewManageDataSourcesPageEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newManageDataSourcesPageEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewManageDataSourcesPageEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNewManageDataSourcesPageEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newManageDataSourcesPageEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewManageDataSourcesPageEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNewManageDataSourcesPageEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newManageDataSourcesPageEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewManageDataSourcesPageEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNewManageDataSourcesPageEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newManageDataSourcesPageEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewManageDataSourcesPageEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsNewProviderCalculationResultsIndexEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newProviderCalculationResultsIndexEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewProviderCalculationResultsIndexEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNewProviderCalculationResultsIndexEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newProviderCalculationResultsIndexEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewProviderCalculationResultsIndexEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNewProviderCalculationResultsIndexEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newProviderCalculationResultsIndexEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewProviderCalculationResultsIndexEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNewProviderCalculationResultsIndexEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newProviderCalculationResultsIndexEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewProviderCalculationResultsIndexEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsNewProviderCalculationResultsIndexEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newProviderCalculationResultsIndexEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewProviderCalculationResultsIndexEnabled();

            // Assert
            result.Should().BeTrue();
        }

		[TestMethod]
		public void IsProviderInformationViewInViewFundingPageEnabled_WhenConfigNotPresent_ReturnsFalse()
		{
			// Arrange
			IConfigurationSection config = Substitute.For<IConfigurationSection>();
			config[Arg.Is("providerInformationViewInViewFundingPageEnabled")].Returns((string)null);

			IFeatureToggle features = new Features(config);

			// Act
			bool result = features.IsProviderInformationViewInViewFundingPageEnabled();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsProviderInformationViewInViewFundingPageEnabled_WhenConfigEmptyString_ReturnsFalse()
		{
			// Arrange
			IConfigurationSection config = Substitute.For<IConfigurationSection>();
			config[Arg.Is("providerInformationViewInViewFundingPageEnabled")].Returns(string.Empty);

			IFeatureToggle features = new Features(config);

			// Act
			bool result = features.IsProviderInformationViewInViewFundingPageEnabled();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsProviderInformationViewInViewFundingPageEnabled_WhenConfigNotBoolean_ReturnsFalse()
		{
			// Arrange
			IConfigurationSection config = Substitute.For<IConfigurationSection>();
			config[Arg.Is("providerInformationViewInViewFundingPageEnabled")].Returns("not a bool");

			IFeatureToggle features = new Features(config);

			// Act
			bool result = features.IsProviderInformationViewInViewFundingPageEnabled();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsProviderInformationViewInViewFundingPageEnabled_WhenConfigFalse_ReturnsFalse()
		{
			// Arrange
			IConfigurationSection config = Substitute.For<IConfigurationSection>();
			config[Arg.Is("providerInformationViewInViewFundingPageEnabled")].Returns("false");

			IFeatureToggle features = new Features(config);

			// Act
			bool result = features.IsProviderInformationViewInViewFundingPageEnabled();

			// Assert
			result.Should().BeFalse();
		}

		[TestMethod]
		public void IsProviderInformationViewInViewFundingPageEnabled_WhenConfigTrue_ReturnsTrue()
		{
			// Arrange
			IConfigurationSection config = Substitute.For<IConfigurationSection>();
			config[Arg.Is("providerInformationViewInViewFundingPageEnabled")].Returns("true");

			IFeatureToggle features = new Features(config);

			// Act
			bool result = features.IsProviderInformationViewInViewFundingPageEnabled();

			// Assert
			result.Should().BeTrue();
		}

        [TestMethod]
        public void IsDynamicBuildProjectEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("dynamicBuildProjectEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsDynamicBuildProjectEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsDynamicBuildProjectEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("dynamicBuildProjectEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsDynamicBuildProjectEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsDynamicBuildProjectEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("dynamicBuildProjectEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsDynamicBuildProjectEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsDynamicBuildProjectEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("dynamicBuildProjectEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsDynamicBuildProjectEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsDynamicBuildProjectEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("dynamicBuildProjectEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsDynamicBuildProjectEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsSearchModeAllEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("searchModeAllEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsSearchModeAllEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsSearchModeAllEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("searchModeAllEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsSearchModeAllEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsSearchModeAllEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("searchModeAllEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsSearchModeAllEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsSearchModeAllEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("searchModeAllEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsSearchModeAllEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsSearchModeAllEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("searchModeAllEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsSearchModeAllEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsUseFieldDefinitionIdsInSourceDatasetsEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("useFieldDefinitionIdsInSourceDatasetsEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsUseFieldDefinitionIdsInSourceDatasetsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsUseFieldDefinitionIdsInSourceDatasetsEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("useFieldDefinitionIdsInSourceDatasetsEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsUseFieldDefinitionIdsInSourceDatasetsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsUseFieldDefinitionIdsInSourceDatasetsEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("useFieldDefinitionIdsInSourceDatasetsEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsUseFieldDefinitionIdsInSourceDatasetsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsUseFieldDefinitionIdsInSourceDatasetsEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("useFieldDefinitionIdsInSourceDatasetsEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsUseFieldDefinitionIdsInSourceDatasetsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsUseFieldDefinitionIdsInSourceDatasetsEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("useFieldDefinitionIdsInSourceDatasetsEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsUseFieldDefinitionIdsInSourceDatasetsEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsProcessDatasetDefinitionNameChangesEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("processDatasetDefinitionNameChangesEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProcessDatasetDefinitionNameChangesEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProcessDatasetDefinitionNameChangesEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("processDatasetDefinitionNameChangesEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProcessDatasetDefinitionNameChangesEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProcessDatasetDefinitionNameChangesEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("processDatasetDefinitionNameChangesEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProcessDatasetDefinitionNameChangesEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProcessDatasetDefinitionNameChangesEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("processDatasetDefinitionNameChangesEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProcessDatasetDefinitionNameChangesEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProcessDatasetDefinitionNameChangesEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("processDatasetDefinitionNameChangesEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProcessDatasetDefinitionNameChangesEnabled();

            // Assert
            result.Should().BeTrue();
        }


        [TestMethod]
        public void IsProcessDatasetDefinitionFieldChangesEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("processDatasetDefinitionFieldChangesEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProcessDatasetDefinitionFieldChangesEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProcessDatasetDefinitionFieldChangesEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("processDatasetDefinitionFieldChangesEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProcessDatasetDefinitionFieldChangesEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProcessDatasetDefinitionFieldChangesEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("processDatasetDefinitionFieldChangesEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProcessDatasetDefinitionFieldChangesEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProcessDatasetDefinitionFieldChangesEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("processDatasetDefinitionFieldChangesEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProcessDatasetDefinitionFieldChangesEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProcessDatasetDefinitionFieldChangesEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("processDatasetDefinitionFieldChangesEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProcessDatasetDefinitionFieldChangesEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsExceptionMessagesEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("exceptionMessagesEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsExceptionMessagesEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsExceptionMessagesEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("exceptionMessagesEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsExceptionMessagesEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsProviderResultsSpecificationCleanupEnabledTrue_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerResultsSpecificationCleanupEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderResultsSpecificationCleanupEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProviderResultsSpecificationCleanupEnabledTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerResultsSpecificationCleanupEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderResultsSpecificationCleanupEnabled();

            // Assert
            result.Should().BeTrue();
        }
    }
}
