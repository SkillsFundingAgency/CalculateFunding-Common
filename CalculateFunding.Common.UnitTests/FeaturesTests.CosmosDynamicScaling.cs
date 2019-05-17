using CalculateFunding.Common.FeatureToggles;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.UnitTests
{
    public partial class FeaturesTests
    {
        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("        ")]
        [DataRow("not a bool")]
        public void IsCosmosDynamicScalingEnabled_WhenInvalidConfigSettingPresent_ReturnsFalse(string configurationSetting)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("cosmosDynamicScalingEnabled")].Returns(configurationSetting);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCosmosDynamicScalingEnabled();

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        [DataRow("true")]
        [DataRow("false")]
        public void IsCosmosDynamicScalingEnabled_WhenValidConfigSettingPresent_EnsuresToggleSetCorrectly(string configurationSetting)
        {
            // Arrange
            IConfigurationSection config = Substitute.For<IConfigurationSection>();
            config[Arg.Is("cosmosDynamicScalingEnabled")].Returns(configurationSetting);

            IFeatureToggle features = new Features(config);

            // Act
            bool result = features.IsCosmosDynamicScalingEnabled();

            // Assert
            result.Should().Be(Boolean.Parse(configurationSetting));
        }
    }
}
