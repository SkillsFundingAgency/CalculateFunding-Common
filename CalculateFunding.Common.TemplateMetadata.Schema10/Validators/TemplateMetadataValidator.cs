using System.Collections.Generic;
using System.Linq;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.TemplateMetadata.Schema10.Models;
using FluentValidation;
using FluentValidation.Validators;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Validators
{
    public class TemplateMetadataValidator : AbstractValidator<FeedBaseModel>
    {
        public TemplateMetadataValidator()
        {
            RuleFor(model => model.Funding.FundingValue)
                .NotEmpty()
                .WithMessage("No funding lines provided for TemplateMetadataValidator")
                .Custom((fundingValue, context) =>
                {
                    TemplateMetadataValidatorContext templateMetadataValidatorContext = new TemplateMetadataValidatorContext();

                    if (!fundingValue.FundingLines.IsNullOrEmpty())
                    {
                        fundingValue.FundingLines.ToList().ForEach(x => ValidateFundingLine(context, x, templateMetadataValidatorContext));
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
                context.AddFailure("FundingLine", $"Funding line name: '{fundingLineName}' is present multiple times in the template but with a different templateLineIds.");
            }

            FundingLine existingFundingLine = validatorContext.FundingLineDictionary.GetOrAdd(fundingLine.TemplateLineId, fundingLine);

            if (!existingFundingLine.FundingLines.IsNullOrEmpty() && !existingFundingLine.FundingLines.All(_ => fundingLine.FundingLines.Any(fl => fl.TemplateLineId == _.TemplateLineId)))
            {
                context.AddFailure("FundingLine", $"FundingLine : '{existingFundingLine.Name}' and id : '{existingFundingLine.TemplateLineId}' has funding line children which don't match.");
            }

            if (!existingFundingLine.Calculations.IsNullOrEmpty() && !existingFundingLine.Calculations.All(_ => fundingLine.Calculations.Any(c => c.TemplateCalculationId == _.TemplateCalculationId)))
            {
                context.AddFailure("FundingLine", $"FundingLine : '{existingFundingLine.Name}' and id : '{existingFundingLine.TemplateLineId}' has child calculations which don't match.");
            }

            if (existingFundingLine.Name.Trim().ToLower() != fundingLineName || existingFundingLine.Type != fundingLine.Type || existingFundingLine.FundingLineCode != fundingLine.FundingLineCode)
            {
                context.AddFailure("FundingLine", $"FundingLine : '{existingFundingLine.Name}' and id : '{existingFundingLine.TemplateLineId}' has funding line items with the same templateLineId that have different configurations for 'name', 'type', 'funding line code'.");
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
            ICollection<uint> existingTemplateCalculationIds = validatorContext.CalculationTemplateCalcIds.GetOrAdd(calculationName, _ => new HashSet<uint>());
            existingTemplateCalculationIds.Add(calculation.TemplateCalculationId);

            if (existingTemplateCalculationIds.Any(_ => _ != calculation.TemplateCalculationId))
            {
                context.AddFailure("Calculation", $"Calculation name: '{calculationName}' is present multiple times in the template but with a different templateCalculationIds.");
            }

            Calculation existingCalculation = validatorContext.CalculationDictionary.GetOrAdd(calculation.TemplateCalculationId, calculation);

            if (!existingCalculation.Calculations.IsNullOrEmpty() && !existingCalculation.Calculations.All(_ => calculation.Calculations.AnyWithNullCheck() && calculation.Calculations.Any(fl => fl.TemplateCalculationId == _.TemplateCalculationId)))
            {
                context.AddFailure("Calculation", $"Calculation : '{existingCalculation.Name}' and id : '{calculation.TemplateCalculationId}' has child calculations which don't match.");
            }

            if (existingCalculation.Name.Trim().ToLower() != calculationName || existingCalculation.AggregationType != calculation.AggregationType || existingCalculation.Type != calculation.Type || existingCalculation.ValueFormat != calculation.ValueFormat || existingCalculation.FormulaText != calculation.FormulaText)
            {
                context.AddFailure("Calculation", $"Calculation : '{existingCalculation.Name}' and id : '{calculation.TemplateCalculationId}' has calculation items with the same templateCalculationId that have different configurations for 'name', 'valueFormat','type'.'formulaText','aggregationType'.");
            }
            else
            {
                if (!existingCalculation.ReferenceData.IsNullOrEmpty())
                {
                    if (!calculation.ReferenceData.IsNullOrEmpty())
                    {
                        if (existingCalculation.ReferenceData.Count() != calculation.ReferenceData.Count() || !existingCalculation.ReferenceData.Any(x => calculation.ReferenceData.Any(y => y.TemplateReferenceId == x.TemplateReferenceId)))
                        {
                            context.AddFailure("ReferenceData", $"Calculation : '{existingCalculation.Name}' and id : '{existingCalculation.TemplateCalculationId}' reference data doesn't match all templateReferenceId's for calculation : '{existingCalculation.Name}' and id : '{existingCalculation.TemplateCalculationId}'.");
                        }
                        else
                        {
                            calculation.ReferenceData?.ToList().ForEach(x => ValidateReferenceData(context, x, existingCalculation, validatorContext));
                        }
                    }
                    else
                    {
                        context.AddFailure("ReferenceData", $"Reference : '{existingCalculation.ReferenceData.First().Name}' doesn't exist on Calculation : {calculationName}.");
                    }
                }
                else
                {
                    if (!calculation.ReferenceData.IsNullOrEmpty())
                    {
                        context.AddFailure("ReferenceData", $"Reference : '{calculation.ReferenceData.First().Name}' doesn't exist on Calculation : {existingCalculation.Name}.");
                    }
                }
            }

            foreach (Calculation nestedCalculation in calculation.Calculations ?? new Calculation[0])
            {
                ValidateCalculation(context, nestedCalculation, validatorContext);
            }
        }

        private void ValidateReferenceData(CustomContext context, ReferenceData referenceData, Calculation existingCalculation, TemplateMetadataValidatorContext validatorContext)
        {
            ReferenceData currentReferenceData = existingCalculation.ReferenceData.Where(x => x.TemplateReferenceId == referenceData.TemplateReferenceId).SingleOrDefault();

            if (currentReferenceData != null)
            {
                ReferenceData existingReferenceData = validatorContext.ReferenceDataDictionary.GetOrAdd(currentReferenceData.TemplateReferenceId, currentReferenceData);

                if (existingReferenceData.AggregationType != referenceData.AggregationType || existingReferenceData.Format != referenceData.Format || existingReferenceData.Name != referenceData.Name)
                {
                    context.AddFailure("ReferenceData", $"Reference : '{referenceData.Name}' and id : '{referenceData.TemplateReferenceId}' has different data items with the same templateReferenceId that have different configuration for 'name', 'format' and 'aggregationType' .");
                }
            }
            else
            {
                // this should never happen as it will have been picked up earlier
                context.AddFailure("ReferenceData", $"Calculation : '{existingCalculation.Name}' and id : '{existingCalculation.TemplateCalculationId}' reference data doesn't match all templateReferenceId's for calculation : '{existingCalculation.Name}' and id : '{existingCalculation.TemplateCalculationId}'.");
            }
        }
    }
}
