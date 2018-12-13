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
    }
}
