using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CalculateFunding.Common.TemplateMetadata.Enums;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.TemplateMetadata.Schema10;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;

namespace CalculateFunding.TemplateMetadata.Schema10.UnitTests
{
    [TestClass]
    public class TemplateMetadataGeneratorTests
    {
        private ILogger logger;
        private TemplateMetadataGenerator templateMetaDataGenerator;

        [TestInitialize]
        public void SetUp()
        {
            logger = Substitute.For<ILogger>();
            templateMetaDataGenerator = new TemplateMetadataGenerator(logger);
        }

        [TestMethod]
        public void TemplateMetadataSchema10_GetInvalidMetaData_ReturnsEmptyContents()
        {
            TemplateMetadataContents contents = WhenTheMetadataContentsIsGenerated("CalculateFunding.TemplateMetadata.Schema10.UnitTests.Resources.dsg1.0.error.json");

            logger
                .Received(1)
                .Error(Arg.Is<Exception>(x => x.GetType().Name == "JsonSerializationException"), Arg.Any<string>());
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema10_ValidMetaDataSupplied_ReturnsValid()
        {
            ValidationResult result = WhenTheTemplateIsValidated("CalculateFunding.TemplateMetadata.Schema10.UnitTests.Resources.dsg1.0.json");

            result.IsValid
                .Should()
                .Be(true);
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema10_ValidMetaDataSuppliedPsg10_ReturnsValid()
        {
            ValidationResult result = WhenTheTemplateIsValidated("CalculateFunding.TemplateMetadata.Schema10.UnitTests.Resources.psg1.0.json");

            result.IsValid
                .Should()
                .Be(true);
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema10_InvalidMetaDataSupplied_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated("CalculateFunding.TemplateMetadata.Schema10.UnitTests.Resources.dsg1.0.invalid.json");

            result.IsValid
                .Should()
                .Be(false);

            result.Errors.First().PropertyName
                .Should()
                .Be("DistributionPeriods");
        }
        
        [TestMethod]
        public void TemplateMetadataValidatorSchema10_DuplicateCalcNameDifferentTemplateCalcId_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated("CalculateFunding.TemplateMetadata.Schema10.UnitTests.Resources.dsg1.0.duplicate.calc.name.json");

            result.IsValid
                .Should()
                .Be(false);

            result.Errors.First().PropertyName
                .Should()
                .Be("Calculation");
            
            result.Errors.First().ErrorMessage
                .Should()
                .StartWith("Calculation name: 'number of pupils' is present multiple times in the template but with a different templateCalculationIds.");
        }

        [TestMethod]
        public void TemplateMetadataSchema10_GetValidMetaData_ReturnsValidContents()
        {
            TemplateMetadataContents contents = WhenTheMetadataContentsIsGenerated("CalculateFunding.TemplateMetadata.Schema10.UnitTests.Resources.dsg1.0.json");

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

        private ValidationResult WhenTheTemplateIsValidated(string templateResourcePath)
        {
            return templateMetaDataGenerator.Validate(GetResourceString(templateResourcePath));
        }

        private TemplateMetadataContents WhenTheMetadataContentsIsGenerated(string templateResourcePath)
        {
            return templateMetaDataGenerator.GetMetadata(GetResourceString(templateResourcePath));
        }

        private string GetResourceString(string resourceName)
        {
            Stream manifestResourceStream = Assembly
                .GetExecutingAssembly()
                .GetManifestResourceStream(resourceName);

            manifestResourceStream
                .Should()
                .NotBeNull($"Expected an embedded resource file at {resourceName}");
            
            using (Stream stream = manifestResourceStream)
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}