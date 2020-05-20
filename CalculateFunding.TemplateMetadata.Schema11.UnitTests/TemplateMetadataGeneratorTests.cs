using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CalculateFunding.Common.TemplateMetadata.Enums;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.TemplateMetadata.Schema11;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;

namespace CalculateFunding.TemplateMetadata.Schema11.UnitTests
{
    [TestClass]
    public class TemplateMetadataGeneratorTests
    {
        private ILogger _logger;
        private TemplateMetadataGenerator _templateMetaDataGenerator;
        private readonly string DsgTemplateWithNoIssues =
            "CalculateFunding.TemplateMetadata.Schema11.UnitTests.Resources.dsg1.0.json";
        private readonly string DsgTemplateWithFundingLinesMissing =
            "CalculateFunding.TemplateMetadata.Schema11.UnitTests.Resources.dsg1.0.fundinglines.missing.json";
        private readonly string DsgTemplateWithDuplicateCalculationNames =
            "CalculateFunding.TemplateMetadata.Schema11.UnitTests.Resources.dsg1.0.duplicate.calc.name.json";

        [TestInitialize]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger>();
            _templateMetaDataGenerator = new TemplateMetadataGenerator(_logger);
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema11_ValidMetaDataSupplied_ReturnsValid()
        {
            var result = WhenTheTemplateIsValidated(DsgTemplateWithNoIssues);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema11_DuplicateCalcNameDifferentTemplateCalcId_ReturnsInvalid()
        {
            var result = WhenTheTemplateIsValidated(DsgTemplateWithDuplicateCalculationNames);

            result.IsValid.Should().BeFalse();

            result.Errors.Count(x => x.PropertyName == "Calculation")
                .Should()
                .Be(1);

            result.Errors.First(x => x.PropertyName == "Calculation").ErrorMessage
                .Should()
                .StartWith("Calculation name: 'calc 11111' is present multiple times in the template but with a different templateCalculationIds.");
        }

        [TestMethod]
        public void TemplateMetadataSchema11_GetValidMetaData_ReturnsValidContents()
        {
            var contents = WhenTheMetadataContentsIsGenerated(DsgTemplateWithNoIssues);

            contents.RootFundingLines.Should().HaveCount(2);

            contents.RootFundingLines.First().Name
                .Should()
                .Be("Funding Line 1");

            contents.RootFundingLines.First().Type
                .Should()
                .Be(FundingLineType.Payment);

            contents.RootFundingLines.Last().Name
                .Should()
                .Be("Funding Line 2");

            contents.RootFundingLines.Last().FundingLineCode
                .Should()
                .Be("DSG-002");

            contents.RootFundingLines.Last().Calculations.First().ValueFormat
                .Should()
                .Be(CalculationValueFormat.Percentage);

            contents.RootFundingLines.Last().Calculations.First().AggregationType
                .Should()
                .Be(AggregationType.Sum);

            contents.RootFundingLines.Last().Calculations.First().Type
                .Should()
                .Be(CalculationType.Weighting);

            contents.SchemaVersion.Should().Be("1.1");
        }

        private ValidationResult WhenTheTemplateIsValidated(string templateResourcePath)
        {
            return _templateMetaDataGenerator.Validate(GetResourceString(templateResourcePath));
        }

        private TemplateMetadataContents WhenTheMetadataContentsIsGenerated(string templateResourcePath)
        {
            return _templateMetaDataGenerator.GetMetadata(GetResourceString(templateResourcePath));
        }

        private string GetResourceString(string resourceName)
        {
            var resources = Assembly
                .GetExecutingAssembly().GetManifestResourceNames();
            var manifestResourceStream = Assembly
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