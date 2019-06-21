using System.Collections.Generic;
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
        private static IEnumerable<object[]> FeatureToggleTestCases()
        {
            yield return new object[] { null, false };
            yield return new object[] { string.Empty, false };
            yield return new object[] { "    ", false };
            yield return new object[] { "not a bool", false };
            yield return new object[] { "false", false };
            yield return new object[] { "FALSE", false };
            yield return new object[] { "FaLSe", false };
            yield return new object[] { "true", true };
            yield return new object[] { "TRUE", true };
            yield return new object[] { "tRue", true };
        }

        /* HACK The Common solution doesn't seem to have any DI at present; if it did this could be simpler still
         * - One test method per property to check that each of the properties called the correct config entry
         * - One test method on the property evaluator to check that it runs through the paths correctly
         * At the moment, for the lack of DI these have to be combined, leading to a lot of repeated testing and excess code
         */

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsProviderProfilingServiceEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerProfilingServiceDisabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderProfilingServiceDisabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsPublishButtonEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("publishButtonEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsPublishButtonEnabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsPublishAndApproveFilterEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("publishAndApprovePageFiltersEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsPublishAndApprovePageFiltersEnabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsNewEditCalculationPageEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newEditCalculationPageEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewEditCalculationPageEnabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsNewManageDataSourcesPageEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newManageDataSourcesPageEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewManageDataSourcesPageEnabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsNewProviderCalculationResultsIndexEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("newProviderCalculationResultsIndexEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsNewProviderCalculationResultsIndexEnabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsProviderInformationViewInViewFundingPageEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("providerInformationViewInViewFundingPageEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProviderInformationViewInViewFundingPageEnabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsDynamicBuildProjectEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("dynamicBuildProjectEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsDynamicBuildProjectEnabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsSearchModeAllEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("searchModeAllEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsSearchModeAllEnabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsUseFieldDefinitionIdsInSourceDatasetsEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("useFieldDefinitionIdsInSourceDatasetsEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsUseFieldDefinitionIdsInSourceDatasetsEnabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsProcessDatasetDefinitionNameChangesEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("processDatasetDefinitionNameChangesEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProcessDatasetDefinitionNameChangesEnabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsProcessDatasetDefinitionFieldChangesEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("processDatasetDefinitionFieldChangesEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsProcessDatasetDefinitionFieldChangesEnabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsExceptionMessagesEnabled_ReturnsAsExpected(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("exceptionMessagesEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsExceptionMessagesEnabled();

            // Assert
            result.Should().Be(output);
        }

#if NCRUNCH
        [Ignore]
#endif
        [TestMethod]
        [DynamicData(nameof(FeatureToggleTestCases), DynamicDataSourceType.Method)]
        public void IsCosmosDynamicScalingEnabled_WhenInvalidConfigSettingPresent_ReturnsFalse(string input, bool output)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("cosmosDynamicScalingEnabled")].Returns(input);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCosmosDynamicScalingEnabled();

            // Assert
            result.Should().Be(output);
        }
    }
}
