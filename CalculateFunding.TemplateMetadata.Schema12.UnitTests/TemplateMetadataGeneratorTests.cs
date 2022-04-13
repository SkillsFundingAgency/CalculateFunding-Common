using System.Collections.Generic;
using System.Linq;
using CalculateFunding.Common.TemplateMetadata.Enums;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.TemplateMetadata.Schema12;
using CalculateFunding.Common.Testing;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Serilog;

namespace CalculateFunding.TemplateMetadata.Schema12.UnitTests
{
    [TestClass]
    public class TemplateMetadataGeneratorTests
    {
        private ILogger _logger;
        private TemplateMetadataGenerator _templateMetaDataGenerator;

        private const string TemplateWithNoIssues =
            "CalculateFunding.TemplateMetadata.Schema12.UnitTests.Resources.template.perfect.json";
        private const string TemplateWithNonMatchingClones =
            "CalculateFunding.TemplateMetadata.Schema12.UnitTests.Resources.template.with.non.matching.clones.json";
        private const string TemplateWithDuplicateCalculationNames =
            "CalculateFunding.TemplateMetadata.Schema12.UnitTests.Resources.template.with.duplicate.calculation.name.json";
        private const string TemplateWithDuplicateAllowedEnumNames =
            "CalculateFunding.TemplateMetadata.Schema12.UnitTests.Resources.template.with.duplicate.allowed.enum.json";
        private const string TemplateWithAnEmptyAllowedEnumName =
            "CalculateFunding.TemplateMetadata.Schema12.UnitTests.Resources.template.with.empty.allowed.enum.value.json";
        private const string TemplateWithANullAllowedEnumName =
            "CalculateFunding.TemplateMetadata.Schema12.UnitTests.Resources.template.with.null.allowed.enum.value.json";
        private const string TemplateWithInvalidGroupRateNumerator =
            "CalculateFunding.TemplateMetadata.Schema12.UnitTests.Resources.template.with.invalid.groupRate.numerator.json";
        private const string TemplateWithGroupRateNumeratorSameAsDenominator =
            "CalculateFunding.TemplateMetadata.Schema12.UnitTests.Resources.template.with.groupRate.numerator.same.as.denominator.json";
        private const string TemplateWithEnumTypeButNoAllowedEnums =
            "CalculateFunding.TemplateMetadata.Schema12.UnitTests.Resources.template.with.enum.type.but.no.allowed.enums.json";
        private const string TemplateWithRecursivePercentageChange =
            "CalculateFunding.TemplateMetadata.Schema12.UnitTests.Resources.template.with.recursive.percentageChange.json";
        private const string TemplateWithPercentageChangeCalculationASameAsCalculationB =
            "CalculateFunding.TemplateMetadata.Schema12.UnitTests.Resources.template.with.percentageChange.CalculationA.same.as.CalculationB.json";
        private const string TemplateWithPaymentFundingLineExceedingMaxAllowedCharacterLimit =
            "CalculateFunding.TemplateMetadata.Schema12.UnitTests.Resources.template.with.payment.fundingline.exceeding.max.allowed.character.limit.json";

        [TestInitialize]
        public void SetUp()
        {
            _logger = Substitute.For<ILogger>();
            _templateMetaDataGenerator = new TemplateMetadataGenerator(_logger);
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema12_ValidMetaDataSupplied_ReturnsValid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(TemplateWithNoIssues);

            result.Errors.Should().BeNullOrEmpty();
            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema12_WithNonMatchingClones_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(TemplateWithNonMatchingClones);

            result.IsValid.Should().BeFalse();

            result.Errors.Where(x => x.PropertyName == "Calculation")
                .Should().HaveCount(1);

            result.Errors.First(x => x.PropertyName == "Calculation").ErrorMessage
                .Should()
                .Be("Calculation with name 'Calculation 2' and id '2' has child calculations which don't match.");
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema12_WithPaymentFundingLineExceedingMaxAllowedCharacterLimit_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(TemplateWithPaymentFundingLineExceedingMaxAllowedCharacterLimit);

            result.IsValid.Should().BeFalse();

            result.Errors.Where(x => x.PropertyName == "FundingLine")
                .Should().HaveCount(1);

            result.Errors.First(x => x.PropertyName == "FundingLine").ErrorMessage
                .Should()
                .Be($"Funding Line 'FundingLine1FundingLine2FundingLine3FundingLine4FundingLine5FundingLine6FundingLine7FundingLine8FundingLine9FundingLine10' with id '1' - Funding line name may not exceed 100 characters in length for payment type lines");
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema12_DuplicateAllowedEnumNames_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(TemplateWithDuplicateAllowedEnumNames);

            result.IsValid.Should().BeFalse();

            result.Errors.Where(x => x.PropertyName == "Calculation")
                .Should().HaveCount(1);

            result.Errors.Single(x => x.PropertyName == "Calculation").ErrorMessage
                .Should()
                .Be("Calculation with name 'Duplicate enum' and id '2' has duplicate allowed enum values for 'enum2', 'enum3'");
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema12_IncludingEmptyAllowedEnumNames_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(TemplateWithAnEmptyAllowedEnumName);

            result.IsValid.Should().BeFalse();

            result.Errors.Where(x => x.PropertyName == "Calculation")
                .Should().HaveCount(1);

            result.Errors.Single(x => x.PropertyName == "Calculation").ErrorMessage
                .Should()
                .Be("Calculation with name 'Duplicate enum' and id '2' contains a null or empty allowed enum value");
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema12_IncludingNullAllowedEnumName_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(TemplateWithANullAllowedEnumName);

            result.IsValid.Should().BeFalse();

            result.Errors.Where(x => x.PropertyName == "Calculation")
                .Should().HaveCount(1);

            result.Errors.Single(x => x.PropertyName == "Calculation").ErrorMessage
                .Should()
                .Be("Calculation with name 'Duplicate enum' and id '2' contains a null or empty allowed enum value");
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema12_MissingAllowedEnumValues_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(TemplateWithEnumTypeButNoAllowedEnums);

            result.IsValid.Should().BeFalse();

            result.Errors.Where(x => x.PropertyName == "Calculation")
                .Should().HaveCount(1);

            result.Errors.First(x => x.PropertyName == "Calculation").ErrorMessage
                .Should()
                .Contain("is of type Enum but has missing allowed enum values");
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema12_InvalidGroupRateNumerator_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(TemplateWithInvalidGroupRateNumerator);

            result.IsValid.Should().BeFalse();

            result.Errors.Where(x => x.PropertyName == "Calculation")
                .Should().HaveCount(1);

            result.Errors.Single(x => x.PropertyName == "Calculation").ErrorMessage
                .Should()
                .Be("Calculation with name 'Calculation 2' and id : '3' has a numerator with TemplateCalculationId 1 that does not refer to a calculation in this template.");
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema12_GroupRateNumeratorSameAsDenominator_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(TemplateWithGroupRateNumeratorSameAsDenominator);

            result.IsValid.Should().BeFalse();

            result.Errors.Where(x => x.PropertyName == "Calculation")
                .Should().HaveCount(1);

            result.Errors.Single(x => x.PropertyName == "Calculation").ErrorMessage
                .Should()
                .Be("Calculation with name 'Calculation 2' and id : '3' has the same numerator & denominator for Group Rate.");
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema12_DuplicateCalcNameDifferentTemplateCalcId_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(TemplateWithDuplicateCalculationNames);

            result.IsValid.Should().BeFalse();

            result.Errors.Count(x => x.PropertyName == "Calculation")
                .Should()
                .Be(1);

            result.Errors.Single(x => x.PropertyName == "Calculation").ErrorMessage
                .Should()
                .StartWith("Calculation : 'Calc 11111' and id : '5' has a duplicate name in the template with a different TemplateCalculationIds.");
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema12_TemplateWithRecursivePercentageChange_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(TemplateWithRecursivePercentageChange);

            result.IsValid.Should().BeFalse();

            result.Errors.Count(x => x.PropertyName == "Calculation")
                .Should()
                .Be(1);

            result.Errors.Single(x => x.PropertyName == "Calculation").ErrorMessage
                .Should()
                .StartWith("Calculation with name 'Calculation 3' and id : '4' has CalculationA with TemplateCalculationId 3 that contains a group rate referring back to this calculation.");
        }

        [TestMethod]
        public void TemplateMetadataValidatorSchema12_TemplateWithPercentageChangeCalculationASameAsCalculationB_ReturnsInvalid()
        {
            ValidationResult result = WhenTheTemplateIsValidated(TemplateWithPercentageChangeCalculationASameAsCalculationB);

            result.IsValid.Should().BeFalse();

            result.Errors.Count(x => x.PropertyName == "Calculation")
                .Should()
                .Be(1);

            result.Errors.Single(x => x.PropertyName == "Calculation").ErrorMessage
                .Should()
                .StartWith("Calculation with name 'Calculation 3' and id : '4' has the same PercentageChangeBetweenAandB value for CalculationA and CalculationB");
        }

        [TestMethod]
        public void TemplateMetadataSchema12_GetValidMetaData_ReturnsValidContents()
        {
            TemplateMetadataContents contents = WhenTheMetadataContentsIsGenerated(TemplateWithNoIssues);

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
                    Name = "Calculation 2",
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
                    Name = "Calculation 3",
                    AggregationType = AggregationType.GroupRate,
                    ValueFormat = CalculationValueFormat.Number,
                    GroupRate = new GroupRate
                    {
                        Denominator = 12,
                        Numerator = 11
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
                    Name = "Calculation 4",
                    AggregationType = AggregationType.PercentageChangeBetweenAandB,
                    ValueFormat = CalculationValueFormat.Percentage,
                    PercentageChangeBetweenAandB = new PercentageChangeBetweenAandB
                    {
                        CalculationA = 3,
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