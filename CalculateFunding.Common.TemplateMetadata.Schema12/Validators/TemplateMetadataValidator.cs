using System;
using System.Collections.Generic;
using System.Linq;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.TemplateMetadata.Schema12.Models;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.Validators;

namespace CalculateFunding.Common.TemplateMetadata.Schema12.Validators
{
    public class TemplateMetadataValidator : AbstractValidator<SchemaJson>
    {
        private const int MaxAllowedPaymentFundingLineNameCharacterLimit = 100;

        private List<ValidationFailure> _failures;
        private List<SchemaJsonFundingLine> _flattenedLines;
        private List<SchemaJsonCalculation> _flattenedCalculations;

        public TemplateMetadataValidator()
        {
            RuleFor(model => model.FundingTemplate)
                .NotNull()
                .WithMessage("No funding stream provided for TemplateMetadataValidator");
            RuleFor(model => model.FundingTemplate.FundingLines)
                .NotEmpty()
                .WithMessage("No funding lines provided for TemplateMetadataValidator")
                .Custom((fundingLines, context) =>
                {
                    _failures = new List<ValidationFailure>();
                    if (fundingLines.AnyWithNullCheck())
                    {
                        _flattenedLines = new List<SchemaJsonFundingLine>();
                        _flattenedCalculations = new List<SchemaJsonCalculation>();

                        FlattenTree(fundingLines);

                        fundingLines.ToList().ForEach(x =>
                            ValidateFundingLine(context, x));
                    }
                });
        }

        private void AddFailure(CustomContext context, string propertyName, string errorMessage)
        {
            if (!_failures.Any(x => x.PropertyName == propertyName && x.ErrorMessage == errorMessage))
            {
                var failure = new ValidationFailure(propertyName ?? string.Empty, errorMessage);
                _failures.Add(failure);
                context.AddFailure(failure);
            }
        }

        private void FlattenTree(IEnumerable<SchemaJsonFundingLine> fundingLineTree)
        {
            if (fundingLineTree == null) return;

            foreach (var fundingLine in fundingLineTree)
            {
                _flattenedLines.Add(fundingLine);

                foreach (var calculation in fundingLine.Calculations)
                {
                    _flattenedCalculations.Add(calculation);
                    FlattenTree(fundingLine.Calculations);
                }

                FlattenTree(fundingLine.FundingLines);
            }
        }

        private void FlattenTree(IEnumerable<SchemaJsonCalculation> fundingCalculationTree)
        {
            if (fundingCalculationTree == null) return;

            foreach (var calculation in fundingCalculationTree)
            {
                _flattenedCalculations.Add(calculation);
                {
                    FlattenTree(calculation.Calculations);
                }
            }
        }

        private void ValidateFundingLine(CustomContext context, SchemaJsonFundingLine fundingLine)
        {
            string fundingLineName = fundingLine.Name.Trim().ToLower();

            CheckForDuplicateNames(context, fundingLine, fundingLineName);

            CheckForPaymentNameLimitIssues(context, fundingLine, fundingLineName);

            var fundingLineClones = _flattenedLines
                .Where(x => x.TemplateLineId == fundingLine.TemplateLineId && x.Id != fundingLine.Id);
            foreach (var clone in fundingLineClones)
            {
                if (!clone.FundingLines.IsNullOrEmpty() &&
                    !clone.FundingLines.All(cloneChild =>
                        fundingLine.FundingLines.Any(child => child.TemplateLineId == cloneChild.TemplateLineId)))
                {
                    AddFailure(context, "FundingLine",
                        $"Funding Line '{fundingLine.Name}' with id '{fundingLine.TemplateLineId}' has clone(s) with child funding lines which don't match.");
                }

                if (!clone.Calculations.IsNullOrEmpty() && !clone.Calculations.All(cloneChild =>
                    fundingLine.Calculations.Any(child => child.TemplateCalculationId == cloneChild.TemplateCalculationId)))
                {
                    AddFailure(context, "FundingLine",
                        $"Funding Line '{fundingLine.Name}' with id '{fundingLine.TemplateLineId}' has clone(s) with child calculations which don't match.");
                }

                if (clone.Name.Trim().ToLower() != fundingLineName || clone.Type != fundingLine.Type ||
                    clone.FundingLineCode != fundingLine.FundingLineCode)
                {
                    AddFailure(context, "FundingLine",
                        $"Funding Line '{fundingLine.Name}' with id '{fundingLine.TemplateLineId}' " +
                        "has clone(s) with different values.");
                }
            }

            fundingLine.FundingLines?.ToList().ForEach(x => ValidateFundingLine(context, x));

            fundingLine.Calculations?.ToList().ForEach(x => ValidateCalculation(context, x));
        }

        private void CheckForPaymentNameLimitIssues(CustomContext context, SchemaJsonFundingLine fundingLine, string fundingLineName)
        {
            if(fundingLine.Type == FundingLineType.Payment)
            {
                if(fundingLineName.Length >= MaxAllowedPaymentFundingLineNameCharacterLimit)
                {
                    AddFailure(context, "FundingLine",
                        $"Funding Line '{fundingLine.Name}' with id '{fundingLine.TemplateLineId}' - Funding line name may not exceed 100 characters in length for payment type lines");
                }
            }
        }

        private void CheckForDuplicateNames(CustomContext context, SchemaJsonFundingLine fundingLine, string fundingLineName)
        {
            int nameUsages = _flattenedLines.Count(x =>
                x.Name.Equals(fundingLineName, StringComparison.OrdinalIgnoreCase) &&
                x.TemplateLineId != fundingLine.TemplateLineId);
            if (nameUsages > 1)
            {
                AddFailure(context, "FundingLine",
                    $"Funding Line '{fundingLine.Name}' with id : '{fundingLine.TemplateLineId}' " +
                    "has a duplicate funding line name in the template with a different templateLineIds.");
            }
        }

        private void ValidateCalculation(CustomContext context, SchemaJsonCalculation calculation)
        {
            calculation.Name = calculation.Name.Trim();

            CheckForUniqueCalculationName(context, calculation);

            CheckForClonesWithDifferentValues(context, calculation);

            CheckForGroupRateIssues(context, calculation);

            CheckForPercentageChangeIssues(context, calculation);

            CheckForEnumIssues(context, calculation);

            if (calculation.Calculations != null)
            {
                foreach (SchemaJsonCalculation nestedCalculation in calculation.Calculations)
                {
                    ValidateCalculation(context, nestedCalculation);
                }
            }
        }

        private void CheckForClonesWithDifferentValues(CustomContext context, SchemaJsonCalculation calculation)
        {
            var calculationClones = _flattenedCalculations
                .Where(x => x.TemplateCalculationId == calculation.TemplateCalculationId && x.Id != calculation.Id);
            foreach (var clone in calculationClones)
            {
                CheckForNonMatchingChildren(context, calculation, clone);

                if (!clone.Name.Trim().Equals(calculation.Name.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    AddFailure(context, "Calculation",
                        $"Calculation with id '{calculation.TemplateCalculationId}' " +
                        $"occurs more than once in the template but with different names ['{calculation.Name}' vs '{clone.Name}']");
                }

                if (clone.AggregationType != calculation.AggregationType)
                {
                    AddFailure(context, "Calculation",
                        $"Calculation with id '{calculation.TemplateCalculationId}' " +
                        "occurs more than once in the template but with different aggregation types " +
                        $"['{calculation.AggregationType}' vs '{clone.AggregationType}']");
                }

                if (clone.Type != calculation.Type)
                {
                    AddFailure(context, "Calculation",
                        $"Calculation with id '{calculation.TemplateCalculationId}' " +
                        "occurs more than once in the template but with different calculation types " +
                        $"['{calculation.Type}' vs '{clone.Type}']");
                }

                if (clone.ValueFormat != calculation.ValueFormat)
                {
                    AddFailure(context, "Calculation",
                        $"Calculation with id '{calculation.TemplateCalculationId}' " +
                        "occurs more than once in the template but with different value formats " +
                        $"['{calculation.ValueFormat}' vs '{clone.ValueFormat}']");
                }

                if (clone.FormulaText != calculation.FormulaText)
                {
                    AddFailure(context, "Calculation",
                        $"Calculation with id '{calculation.TemplateCalculationId}' " +
                        "occurs more than once in the template but with different formula texts " +
                        $"['{calculation.FormulaText}' vs '{clone.FormulaText}']");
                }

                switch (calculation.AggregationType)
                {
                    case AggregationType.PercentageChangeBetweenAandB
                        when clone.PercentageChangeBetweenAandB.CalculationA != calculation.PercentageChangeBetweenAandB.CalculationA ||
                             clone.PercentageChangeBetweenAandB.CalculationB != calculation.PercentageChangeBetweenAandB.CalculationB ||
                             clone.PercentageChangeBetweenAandB.CalculationAggregationType !=
                             calculation.PercentageChangeBetweenAandB.CalculationAggregationType:
                        AddFailure(context, "Calculation",
                            $"Calculation with id '{calculation.TemplateCalculationId}' " +
                            "occurs more than once in the template but with different values for percentage change between A and B");
                        break;
                    case AggregationType.GroupRate
                        when clone.GroupRate?.Numerator != calculation.GroupRate?.Numerator ||
                             clone.GroupRate?.Denominator != calculation.GroupRate?.Denominator:
                        AddFailure(context, "Calculation",
                            $"Calculation with id '{calculation.TemplateCalculationId}' " +
                            "occurs more than once in the template but with different group rates");
                        break;
                }

                if (!clone.AllowedEnumTypeValues.IsNullOrEmpty() && !clone.AllowedEnumTypeValues.SequenceEqual(calculation.AllowedEnumTypeValues))
                {
                    AddFailure(context, "Calculation",
                        $"Calculation with id '{calculation.TemplateCalculationId}' " +
                        "occurs more than once in the template but with different allowed enum type values " +
                        $"['{calculation.AllowedEnumTypeValues}' vs '{clone.AllowedEnumTypeValues}']");
                }
            }
        }

        private void CheckForNonMatchingChildren(CustomContext context, SchemaJsonCalculation calculation, SchemaJsonCalculation clone)
        {
            if (!clone.Calculations.IsNullOrEmpty() &&
                !clone.Calculations.All(_ =>
                    calculation.Calculations.AnyWithNullCheck() &&
                    calculation.Calculations.Any(fl => fl.TemplateCalculationId == _.TemplateCalculationId)))
            {
                AddFailure(context, "Calculation",
                    $"Calculation with name '{clone.Name}' and id '{calculation.TemplateCalculationId}' " +
                    "has child calculations which don't match.");
            }
        }

        private void CheckForGroupRateIssues(CustomContext context, SchemaJsonCalculation calculation)
        {
            if (calculation.AggregationType == AggregationType.GroupRate)
            {
                if (calculation.GroupRate == null)
                {
                    AddFailure(context, "Calculation",
                        $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                        "has an aggregation type of Group Rate but is missing numerator & denominator.");
                }
                else
                {
                    var numeratorCalculationMatches = _flattenedCalculations
                        .Count(c => c.TemplateCalculationId == calculation.GroupRate.Numerator);
                    if (numeratorCalculationMatches == 0)
                    {
                        AddFailure(context, "Calculation",
                            $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                            $"has a numerator with TemplateCalculationId {calculation.GroupRate.Numerator} " +
                            "that does not refer to a calculation in this template.");
                    }

                    var denominatorCalculationMatches = _flattenedCalculations
                        .Count(c => c.TemplateCalculationId == calculation.GroupRate.Denominator);
                    if (denominatorCalculationMatches == 0)
                    {
                        AddFailure(context, "Calculation",
                            $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                            $"has a denominator with TemplateCalculationId {calculation.GroupRate.Denominator} " +
                            "that does not refer to a calculation in this template.");
                    }

                    if (calculation.GroupRate.Numerator == calculation.GroupRate.Denominator)
                    {
                        AddFailure(context, "Calculation",
                            $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                            "has the same numerator & denominator for Group Rate.");
                    }

                    if (calculation.GroupRate.Numerator == calculation.TemplateCalculationId)
                    {
                        AddFailure(context, "Calculation",
                            $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                            "has a Group Rate numerator referring to its own TemplateCalculationId.");
                    }

                    if (calculation.GroupRate.Denominator == calculation.TemplateCalculationId)
                    {
                        AddFailure(context, "Calculation",
                            $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                            "has a Group Rate denominator referring to its own TemplateCalculationId.");
                    }
                }
            }
        }

        private void CheckForPercentageChangeIssues(CustomContext context, SchemaJsonCalculation calculation)
        {
            if (calculation.AggregationType == AggregationType.PercentageChangeBetweenAandB)
            {
                if (calculation.PercentageChangeBetweenAandB == null)
                {
                    AddFailure(context, "Calculation",
                        $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                        "has an aggregation type of PercentageChangeBetweenAandB but is missing numerator & denominator.");
                }
                else
                {
                    var calculationAMatches = _flattenedCalculations
                        .Where(c => c.TemplateCalculationId == calculation.PercentageChangeBetweenAandB.CalculationA);
                    if (!calculationAMatches.Any())
                    {
                        AddFailure(context, "Calculation",
                            $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                            $"has CalculationA with TemplateCalculationId {calculation.PercentageChangeBetweenAandB.CalculationA} " +
                            "that does not refer to a calculation in this template.");
                    }
                    else
                    {
                        if (calculationAMatches.Any(x =>
                            x.AggregationType == AggregationType.GroupRate &&
                            x.GroupRate != null &&
                            (x.GroupRate.Numerator == calculation.TemplateCalculationId ||
                             x.GroupRate.Denominator == calculation.TemplateCalculationId)))
                        {
                            AddFailure(context, "Calculation",
                                $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                                $"has CalculationA with TemplateCalculationId {calculation.PercentageChangeBetweenAandB.CalculationA} " +
                                "that contains a group rate referring back to this calculation.");
                        }

                        if (calculationAMatches.Any(x =>
                            x.AggregationType == AggregationType.PercentageChangeBetweenAandB &&
                            x.PercentageChangeBetweenAandB != null &&
                            (x.PercentageChangeBetweenAandB.CalculationA == calculation.TemplateCalculationId ||
                             x.PercentageChangeBetweenAandB.CalculationB == calculation.TemplateCalculationId)))
                        {
                            AddFailure(context, "Calculation",
                                $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                                $"has CalculationA with TemplateCalculationId {calculation.PercentageChangeBetweenAandB.CalculationA} " +
                                "that contains a PercentageChangeBetweenAandB referring back to this calculation.");
                        }
                    }

                    var calculationBMatches = _flattenedCalculations
                        .Where(c => c.TemplateCalculationId == calculation.PercentageChangeBetweenAandB.CalculationB);
                    if (!calculationBMatches.Any())
                    {
                        AddFailure(context, "Calculation",
                            $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                            $"has CalculationB with TemplateCalculationId {calculation.PercentageChangeBetweenAandB.CalculationB} " +
                            "that does not refer to a calculation in this template.");
                    }
                    else
                    {
                        if (calculationBMatches.Any(x =>
                            x.AggregationType == AggregationType.GroupRate &&
                            x.GroupRate != null &&
                            (x.GroupRate.Numerator == calculation.TemplateCalculationId ||
                             x.GroupRate.Denominator == calculation.TemplateCalculationId)))
                        {
                            AddFailure(context, "Calculation",
                                $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                                $"has CalculationB with TemplateCalculationId {calculation.PercentageChangeBetweenAandB.CalculationB} " +
                                "that contains a group rate referring back to this calculation.");
                        }

                        if (calculationBMatches.Any(x =>
                            x.AggregationType == AggregationType.PercentageChangeBetweenAandB &&
                            x.PercentageChangeBetweenAandB != null &&
                            (x.PercentageChangeBetweenAandB.CalculationA == calculation.TemplateCalculationId ||
                             x.PercentageChangeBetweenAandB.CalculationB == calculation.TemplateCalculationId)))
                        {
                            AddFailure(context, "Calculation",
                                $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                                $"has CalculationB with TemplateCalculationId {calculation.PercentageChangeBetweenAandB.CalculationB} " +
                                "that contains a PercentageChangeBetweenAandB referring back to this calculation.");
                        }
                    }

                    if (calculation.PercentageChangeBetweenAandB.CalculationA == calculation.PercentageChangeBetweenAandB.CalculationB)
                    {
                        AddFailure(context, "Calculation",
                            $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                            "has the same PercentageChangeBetweenAandB value for CalculationA and CalculationB.");
                    }

                    if (calculation.PercentageChangeBetweenAandB.CalculationA == calculation.TemplateCalculationId)
                    {
                        AddFailure(context, "Calculation",
                            $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                            "has PercentageChangeBetweenAandB CalculationA referring to its own TemplateCalculationId.");
                    }

                    if (calculation.PercentageChangeBetweenAandB.CalculationB == calculation.TemplateCalculationId)
                    {
                        AddFailure(context, "Calculation",
                            $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                            "has PercentageChangeBetweenAandB CalculationB referring to its own TemplateCalculationId.");
                    }
                }
            }
        }

        private void CheckForEnumIssues(CustomContext context, SchemaJsonCalculation calculation)
        {
            if (calculation.Type == FundingCalculationType.Enum)
            {
                if (calculation.AllowedEnumTypeValues.IsNullOrEmpty())
                {
                    AddFailure(context, "Calculation",
                        $"Calculation with name '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                        "is of type Enum but has missing allowed enum values.");
                }
                else
                {
                    if (calculation.AllowedEnumTypeValues.Any(x => x.IsNullOrEmpty()))
                    {
                        AddFailure(context, "Calculation",
                            $"Calculation with name '{calculation.Name}' and id '{calculation.TemplateCalculationId}' " +
                            "contains a null or empty allowed enum value");
                    }

                    var duplicates = calculation.AllowedEnumTypeValues
                        .Select((t, i) => new { Index = i, Text = t })
                        .GroupBy(g => g.Text)
                        .Where(g => g.Count() > 1)
                        .Select(x => x.Key)
                        .ToList();
                    if (duplicates.Any())
                    {
                        AddFailure(context, "Calculation",
                            $"Calculation with name '{calculation.Name}' and id '{calculation.TemplateCalculationId}' " +
                            $"has duplicate allowed enum values for '{string.Join("', '", duplicates)}'");
                    }
                }
            }
        }

        private void CheckForUniqueCalculationName(CustomContext context, SchemaJsonCalculation calculation)
        {
            int nameUsages = _flattenedCalculations.Count(x =>
                x.Name.Equals(calculation.Name, StringComparison.OrdinalIgnoreCase) &&
                x.TemplateCalculationId != calculation.TemplateCalculationId);
            if (nameUsages > 1)
            {
                AddFailure(context, "Calculation",
                    $"Calculation : '{calculation.Name}' and id : '{calculation.TemplateCalculationId}' " +
                    "has a duplicate name in the template with a different TemplateCalculationIds.");
            }
        }
    }
}