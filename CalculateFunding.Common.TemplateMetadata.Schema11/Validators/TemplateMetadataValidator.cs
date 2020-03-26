using System.Collections.Generic;
using System.Linq;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.TemplateMetadata.Schema11.Mapping;
using CalculateFunding.Common.TemplateMetadata.Schema11.Models;
using FluentValidation;
using FluentValidation.Validators;

namespace CalculateFunding.Common.TemplateMetadata.Schema11.Validators
{
    public class TemplateMetadataValidator : AbstractValidator<SchemaJson>
    {
        public TemplateMetadataValidator()
        {
            RuleFor(model => model.FundingStreamTemplate)
                .NotNull()
                .WithMessage("No funding stream provided for TemplateMetadataValidator");
            RuleFor(model => model.FundingStreamTemplate.FundingLines)
                .NotEmpty()
                .WithMessage("No funding lines provided for TemplateMetadataValidator")
                .Custom((fundingLines, context) =>
                {
                    if (fundingLines.AnyWithNullCheck())
                    {
                        fundingLines.ToList().ForEach(x => 
                            ValidateFundingLine(context, x.ToFundingLine(), new TemplateMetadataValidatorContext()));
                    }
                });
        }

        private void ValidateFundingLine(CustomContext context, FundingLine fundingLine, TemplateMetadataValidatorContext validatorContext)
        {
            string fundingLineName = fundingLine.Name.Trim().ToLower();
            ICollection<uint> existingTemplateLineIds = validatorContext.FundingLineTemplateIds.GetOrAdd(fundingLineName, _ => new HashSet<uint>());
            existingTemplateLineIds.Add(fundingLine.TemplateLineId);

            if (existingTemplateLineIds.Any(_ => _ != fundingLine.TemplateLineId))
            {
                context.AddFailure("FundingLine",
                    $"Funding line name: '{fundingLineName}' is present multiple times in the template but with a different templateLineIds.");
            }

            FundingLine existingFundingLine = validatorContext.FundingLineDictionary.GetOrAdd(fundingLine.TemplateLineId, fundingLine);

            if (!existingFundingLine.FundingLines.IsNullOrEmpty() &&
                !existingFundingLine.FundingLines.All(_ => fundingLine.FundingLines.Any(fl => fl.TemplateLineId == _.TemplateLineId)))
            {
                context.AddFailure("FundingLine",
                    $"FundingLine : '{existingFundingLine.Name}' and id : '{existingFundingLine.TemplateLineId}' has funding line children which don't match.");
            }

            if (!existingFundingLine.Calculations.IsNullOrEmpty() && !existingFundingLine.Calculations.All(_ =>
                fundingLine.Calculations.Any(c => c.TemplateCalculationId == _.TemplateCalculationId)))
            {
                context.AddFailure("FundingLine",
                    $"FundingLine : '{existingFundingLine.Name}' and id : '{existingFundingLine.TemplateLineId}' has child calculations which don't match.");
            }

            if (existingFundingLine.Name.Trim().ToLower() != fundingLineName || existingFundingLine.Type != fundingLine.Type ||
                existingFundingLine.FundingLineCode != fundingLine.FundingLineCode)
            {
                context.AddFailure("FundingLine",
                    $"FundingLine : '{existingFundingLine.Name}' and id : '{existingFundingLine.TemplateLineId}' has funding line items with the same templateLineId that have different configurations for 'name', 'type', 'funding line code'.");
            }

            if (!fundingLine.DistributionPeriods.IsNullOrEmpty())
            {
                context.AddFailure("DistributionPeriods", $"Funding line : '{fundingLine.Name}' has values for the distribution periods");
            }

            fundingLine.FundingLines?.ToList().ForEach(x => ValidateFundingLine(context, x, validatorContext));

            fundingLine.Calculations?.ToList().ForEach(x => ValidateCalculation(context, x, validatorContext));
        }

        private void ValidateCalculation(CustomContext context, Calculation calculation, TemplateMetadataValidatorContext validatorContext)
        {
            string calculationName = calculation.Name.Trim().ToLower();
            ICollection<uint> existingTemplateCalculationIds =
                validatorContext.CalculationTemplateCalcIds.GetOrAdd(calculationName, _ => new HashSet<uint>());
            existingTemplateCalculationIds.Add(calculation.TemplateCalculationId);

            if (existingTemplateCalculationIds.Any(_ => _ != calculation.TemplateCalculationId))
            {
                context.AddFailure("Calculation",
                    $"Calculation name: '{calculationName}' is present multiple times in the template but with a different templateCalculationIds.");
            }

            var existingCalculation = validatorContext.CalculationDictionary.GetOrAdd(calculation.TemplateCalculationId, calculation);

            if (!existingCalculation.Calculations.IsNullOrEmpty() && 
                !existingCalculation.Calculations.All(_ =>
                    calculation.Calculations.AnyWithNullCheck() &&
                    calculation.Calculations.Any(fl => fl.TemplateCalculationId == _.TemplateCalculationId)))
            {
                context.AddFailure("Calculation",
                    $"Calculation : '{existingCalculation.Name}' and id : '{calculation.TemplateCalculationId}' has child calculations which don't match.");
            }

            if (existingCalculation.Name.Trim().ToLower() != calculationName || 
                existingCalculation.AggregationType != calculation.AggregationType ||
                existingCalculation.Type != calculation.Type || 
                existingCalculation.ValueFormat != calculation.ValueFormat ||
                existingCalculation.FormulaText != calculation.FormulaText)
            {
                context.AddFailure("Calculation",
                    $"Calculation : '{existingCalculation.Name}' and id : '{calculation.TemplateCalculationId}' has calculation items with the same templateCalculationId that have different configurations for 'name', 'valueFormat','type'.'formulaText','aggregationType'.");
            }

            foreach (Calculation nestedCalculation in calculation.Calculations ?? new Calculation[0])
            {
                ValidateCalculation(context, nestedCalculation, validatorContext);
            }
        }
    }
}