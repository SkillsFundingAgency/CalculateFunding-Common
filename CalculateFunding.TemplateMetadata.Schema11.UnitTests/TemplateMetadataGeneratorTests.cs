using System.Collections.Generic;
using System.Linq;
using CalculateFunding.Common.TemplateMetadata.Enums;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.TemplateMetadata.Schema11;
using CalculateFunding.Common.Testing;
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

        private const string DsgTemplateWithNoIssues =
            "CalculateFunding.TemplateMetadata.Schema11.UnitTests.Resources.dsg1.0.json";

        private const string DsgTemplateWithDuplicateCalculationNames =
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
            ValidationResult result = WhenTheTemplateIsValidated(DsgTemplateWithNoIssues);

            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema11_DuplicateCalcNameDifferentTemplateCalcId_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(DsgTemplateWithDuplicateCalculationNames);

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
            TemplateMetadataContents contents = WhenTheMetadataContentsIsGenerated(DsgTemplateWithNoIssues);

            contents.RootFundingLines.Should().HaveCount(2);

            FundingLine fundingLineOne = contents.RootFundingLines.First();

            fundingLineOne.Name
                .Should()
                .Be("Funding Line 1");

            fundingLineOne.Type
                .Should()
                .Be(FundingLineType.Payment);

            FundingLine fundingLineTwo = contents.RootFundingLines.Last();

            fundingLineTwo.Name
                .Should()
                .Be("Funding Line 2");

            fundingLineTwo.FundingLineCode
                .Should()
                .Be("DSG-002");

            IEnumerable<Calculation> calculations = fundingLineOne.Calculations;

            calculations
                .Should()
                .HaveCount(3);

            Calculation allowedEnumsCalculation = calculations.SingleOrDefault(_
                => _.Type == CalculationType.Enum);

            allowedEnumsCalculation
                .Should()
                .BeEquivalentTo(new Calculation
                {
                    TemplateCalculationId = 2,
                    Name = "Calculation 1",
                    AggregationType = AggregationType.Sum,
                    ValueFormat = CalculationValueFormat.Number,
                    AllowedEnumTypeValues = new[]
                    {
                        "Option1", "Option2"
                    },
                    Type = CalculationType.Enum,
                    FormulaText = "Enter formula text"
                },
                    opt
                        => opt.Excluding(_ => _.Calculations));

            Calculation groupRateCalculation = calculations.SingleOrDefault(_
                => _.AggregationType == AggregationType.GroupRate);

            groupRateCalculation
                .Should()
                .BeEquivalentTo(new Calculation
                {
                    TemplateCalculationId = 3,
                    Name = "Calculation 2",
                    AggregationType = AggregationType.GroupRate,
                    ValueFormat = CalculationValueFormat.Number,
                    GroupRate = new GroupRate
                    {
                        Denominator = 2,
                        Numerator = 1
                    },
                    Type = CalculationType.Boolean,
                    FormulaText = "Enter formula text"
                },
                    opt
                        => opt.Excluding(_ => _.Calculations));

            Calculation percentageChangeCalculation = calculations.SingleOrDefault(_
                => _.AggregationType == AggregationType.PercentageChangeBetweenAandB);

            percentageChangeCalculation
                .Should()
                .BeEquivalentTo(new Calculation
                {
                    TemplateCalculationId = 4,
                    Name = "Calculation 3",
                    AggregationType = AggregationType.PercentageChangeBetweenAandB,
                    ValueFormat = CalculationValueFormat.Percentage,
                    PercentageChangeBetweenAandB = new PercentageChangeBetweenAandB
                    {
                        CalculationA = 1,
                        CalculationB = 2,
                        CalculationAggregationType = AggregationType.Sum
                    },
                    Type = CalculationType.Number,
                    FormulaText = "Enter formula text"
                },
                    opt
                        => opt.Excluding(_ => _.Calculations));

            contents.SchemaVersion
                .Should()
                .Be("1.1");
        }

        private ValidationResult WhenTheTemplateIsValidated(string templateResourcePath)
            => _templateMetaDataGenerator.Validate(GetResourceString(templateResourcePath));

        private TemplateMetadataContents WhenTheMetadataContentsIsGenerated(string templateResourcePath)
            => _templateMetaDataGenerator.GetMetadata(GetResourceString(templateResourcePath));

        private string GetResourceString(string resourceName)
            => typeof(TemplateMetadataGeneratorTests).Assembly
                .GetEmbeddedResourceFileContents(resourceName);
    }
}