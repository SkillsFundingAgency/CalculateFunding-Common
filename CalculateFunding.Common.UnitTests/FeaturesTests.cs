using CalculateFunding.Common.FeatureToggles;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace CalculateFunding.Common.UnitTests
{
    [TestClass]
    public class FeaturesTests
    {
        [TestMethod]
        public void WhenConfigNull_ReturnsFalse()
        {
            // Arrange
            IFeatureToggle features = new Features(null);

            // Act
            bool result = features.IsAllocationLineMajorMinorVersioningEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAllocationLineMajorMinorVersioningEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("allocationLineMajorMinorVersioningEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAllocationLineMajorMinorVersioningEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAllocationLineMajorMinorVersioningEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("allocationLineMajorMinorVersioningEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAllocationLineMajorMinorVersioningEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAllocationLineMajorMinorVersioningEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("allocationLineMajorMinorVersioningEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAllocationLineMajorMinorVersioningEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAllocationLineMajorMinorVersioningEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("allocationLineMajorMinorVersioningEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAllocationLineMajorMinorVersioningEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAllocationLineMajorMinorVersioningEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("allocationLineMajorMinorVersioningEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAllocationLineMajorMinorVersioningEnabled();

            // Assert
            result.Should().BeTrue();
        }

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
        public void IsAggregateSupportInCalculationsEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("aggregateSupportInCalculationsEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAggregateSupportInCalculationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAggregateSupportInCalculationsEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("aggregateSupportInCalculationsEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAggregateSupportInCalculationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAggregateSupportInCalculationsEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("aggregateSupportInCalculationsEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAggregateSupportInCalculationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAggregateSupportInCalculationsEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("aggregateSupportInCalculationsEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAggregateSupportInCalculationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAggregateSupportInCalculationsEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("aggregateSupportInCalculationsEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAggregateSupportInCalculationsEnabled();

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
        public void IsJobServiceEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsJobServiceEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsJobServiceEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsJobServiceEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsJobServiceEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceEnabled();

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
        public void IsAggregateOverCalculationsEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("aggregateOverCalculationsEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAggregateOverCalculationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAggregateOverCalculationsEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("aggregateOverCalculationsEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAggregateOverCalculationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAggregateOverCalculationsEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("aggregateOverCalculationsEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAggregateOverCalculationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAggregateOverCalculationsEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("aggregateOverCalculationsEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAggregateOverCalculationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAggregateOverCalculationsEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("aggregateOverCalculationsEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAggregateOverCalculationsEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsCalculationResultsNotificationsEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("calculationResultsNotificationsEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCalculationResultsNotificationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCalculationResultsNotificationsEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("calculationResultsNotificationsEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCalculationResultsNotificationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCalculationResultsNotificationsEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("calculationResultsNotificationsEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCalculationResultsNotificationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCalculationResultsNotificationsEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("calculationResultsNotificationsEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCalculationResultsNotificationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCalculationResultsNotificationsEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("calculationResultsNotificationsEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCalculationResultsNotificationsEnabled();

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
        public void IsJobServiceForMainActionsEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceForMainActionsEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceForMainActionsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsJobServiceForMainActionsEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceForMainActionsEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceForMainActionsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsJobServiceForMainActionsEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceForMainActionsEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceForMainActionsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsJobServiceForMainActionsEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceForMainActionsEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceForMainActionsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsJobServiceForMainActionsEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceForMainActionsEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceForMainActionsEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsAllAllocationResultsVersionsInFeedIndexEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("allAllocationResultsVersionsInFeedIndexEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAllAllocationResultsVersionsInFeedIndexEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAllAllocationResultsVersionsInFeedIndexEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("allAllocationResultsVersionsInFeedIndexEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAllAllocationResultsVersionsInFeedIndexEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAllAllocationResultsVersionsInFeedIndexEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("allAllocationResultsVersionsInFeedIndexEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAllAllocationResultsVersionsInFeedIndexEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAllAllocationResultsVersionsInFeedIndexEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("allAllocationResultsVersionsInFeedIndexEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAllAllocationResultsVersionsInFeedIndexEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsAllAllocationResultsVersionsInFeedIndexEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("allAllocationResultsVersionsInFeedIndexEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsAllAllocationResultsVersionsInFeedIndexEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsApprovalBatchingServerSideEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("approvalBatchingServerSideEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsApprovalBatchingServerSideEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsApprovalBatchingServerSideEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("approvalBatchingServerSideEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsApprovalBatchingServerSideEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsApprovalBatchingServerSideEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("approvalBatchingServerSideEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsApprovalBatchingServerSideEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsApprovalBatchingServerSideEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("approvalBatchingServerSideEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsApprovalBatchingServerSideEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsApprovalBatchingServerSideEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("approvalBatchingServerSideEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsApprovalBatchingServerSideEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsJobServiceForPublishProviderResultsEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceForPublishProviderResultsEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceForPublishProviderResultsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsJobServiceForPublishProviderResultsEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceForPublishProviderResultsEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceForPublishProviderResultsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsJobServiceForPublishProviderResultsEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceForPublishProviderResultsEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceForPublishProviderResultsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsJobServiceForPublishProviderResultsEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceForPublishProviderResultsEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceForPublishProviderResultsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsJobServiceForPublishProviderResultsEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("jobServiceForPublishProviderResultsEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsJobServiceForPublishProviderResultsEnabled();

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void IsProviderVariationsEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerVariationsEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderVariationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProviderVariationsEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerVariationsEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderVariationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProviderVariationsEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerVariationsEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderVariationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProviderVariationsEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerVariationsEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderVariationsEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsProviderVariationsEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerVariationsEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderVariationsEnabled();

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
        public void IsCheckJobStatusForChooseAndRefreshEnabled_WhenConfigNotPresent_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("checkJobStatusForChooseAndRefreshEnabled")].Returns((string)null);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCheckJobStatusForChooseAndRefreshEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCheckJobStatusForChooseAndRefreshEnabled_WhenConfigEmptyString_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("checkJobStatusForChooseAndRefreshEnabled")].Returns(string.Empty);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCheckJobStatusForChooseAndRefreshEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCheckJobStatusForChooseAndRefreshEnabled_WhenConfigNotBoolean_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("checkJobStatusForChooseAndRefreshEnabled")].Returns("not a bool");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCheckJobStatusForChooseAndRefreshEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCheckJobStatusForChooseAndRefreshEnabled_WhenConfigFalse_ReturnsFalse()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("checkJobStatusForChooseAndRefreshEnabled")].Returns("false");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCheckJobStatusForChooseAndRefreshEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public void IsCheckJobStatusForChooseAndRefreshEnabled_WhenConfigTrue_ReturnsTrue()
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("checkJobStatusForChooseAndRefreshEnabled")].Returns("true");

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCheckJobStatusForChooseAndRefreshEnabled();

            // Assert
            result.Should().BeTrue();
        }
    }
}
