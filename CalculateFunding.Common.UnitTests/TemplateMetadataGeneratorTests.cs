using CalculateFunding.Common.TemplateMetadata;
using CalculateFunding.Common.TemplateMetadata.Enums;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.TemplateMetadata.Schema10;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CalculateFunding.Common.UnitTests
{
    [TestClass]
    public class TemplateMetadataGeneratorTests
    {
        [TestMethod]
        public void TemplateMetadataSchema10_GetInvalidMetaData_ReturnsEmptyContents()
        {
            //Arrange
            ILogger logger = CreateLogger();

            ITemplateMetadataGenerator templateMetaDataGenerator = CreateTemplateGenerator(logger);

            //Act
            TemplateMetadataContents contents = templateMetaDataGenerator.GetMetadata(GetResourceString("CalculateFunding.Common.UnitTests.Resources.dsg1.0.error.json"));

            //Assert
            logger
                .Received(1)
                .Error(Arg.Is<Exception>(x => x.GetType().Name == "JsonSerializationException"), Arg.Any<string>());
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema10_ValidMetaDataSupplied_ReturnsValid()
        {
            //Arrange
            ILogger logger = CreateLogger();

            ITemplateMetadataGenerator templateMetaDataGenerator = CreateTemplateGenerator(logger);

            ValidationResult result = templateMetaDataGenerator.Validate(GetResourceString("CalculateFunding.Common.UnitTests.Resources.dsg1.0.json"));

            result.IsValid
                .Should()
                .Be(true);
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema10_InvalidMetaDataSupplied_ReturnsInvalid()
        {
            //Arrange
            ILogger logger = CreateLogger();

            ITemplateMetadataGenerator templateMetaDataGenerator = CreateTemplateGenerator(logger);

            ValidationResult result = templateMetaDataGenerator.Validate(GetResourceString("CalculateFunding.Common.UnitTests.Resources.dsg1.0.invalid.json"));

            result.IsValid
                .Should()
                .Be(false);

            result.Errors.First().PropertyName
                .Should()
                .Be("DistributionPeriods");
        }

        [TestMethod]
        public void TemplateMetadataSchema10_GetValidMetaData_ReturnsValidContents()
        {
            //Arrange
            ITemplateMetadataGenerator templateMetaDataGenerator = CreateTemplateGenerator();

            //Act
            TemplateMetadataContents contents = templateMetaDataGenerator.GetMetadata(GetResourceString("CalculateFunding.Common.UnitTests.Resources.dsg1.0.json"));

            //Assert
            contents.RootFundingLines.Count()
                .Should()
                .Be(2);

            contents.RootFundingLines.First().Name
                .Should()
                .Be("Prior To Recoupment");

            contents.RootFundingLines.First().Type
                .Should()
                .Be(FundingLineType.Information);

            contents.RootFundingLines.Last().Name
                .Should()
                .Be("Post Deduction For Recoupment And High Needs");

            contents.RootFundingLines.Last().FundingLineCode
                .Should()
                .Be("PostDeductionForRecoupmentAndHighNeeds");

            contents.RootFundingLines.Last().Calculations.First().ValueFormat
                .Should()
                .Be(CalculationValueFormat.Number);

            contents.RootFundingLines.Last().Calculations.First().AggregationType
                .Should()
                .Be(AggregationType.Sum);

            contents.RootFundingLines.Last().Calculations.First().Type
                .Should()
                .Be(CalculationType.PupilNumber);

            contents.RootFundingLines.Last().Calculations.First().ReferenceData.First().AggregationType
                .Should()
                .Be(AggregationType.Sum);

            contents.RootFundingLines.Last().Calculations.First().ReferenceData.First().Format
                .Should()
                .Be(ReferenceDataValueFormat.Number);

            contents.SchemaVersion
                .Should()
                .Be("1.0");
        }

        public ITemplateMetadataGenerator CreateTemplateGenerator(ILogger logger = null)
        {
            return new TemplateMetadataGenerator(logger ?? CreateLogger());
        }

        public ILogger CreateLogger()
        {
            return Substitute.For<ILogger>();
        }

        public string GetResourceString(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
