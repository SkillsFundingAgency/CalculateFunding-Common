using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.TemplateMetadata.Schema10.Models;
using FluentValidation;
using FluentValidation.Validators;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CalculateFunding.Common.TemplateMetadata.Schema10.Validators
{
    public class TemplateMetadataValidator : AbstractValidator<FeedBaseModel>
    {
        private readonly ConcurrentDictionary<uint, Calculation> _calculationDictionary;
        private readonly ConcurrentDictionary<uint, ReferenceData> _referenceDataDictionary;
        private readonly ConcurrentDictionary<string, ICollection<uint>> _calculationTemplateCalcIds = new ConcurrentDictionary<string, ICollection<uint>>();

        public TemplateMetadataValidator()
        {
            _calculationDictionary = new ConcurrentDictionary<uint, Calculation>();
            _referenceDataDictionary = new ConcurrentDictionary<uint, ReferenceData>();

            RuleFor(model => model.Funding.FundingValue)
                .NotEmpty()
                .WithMessage("No funding lines provided for TemplateMetadataValidator")
                .Custom((fundingValue, context) =>
                {
                    if (!fundingValue.FundingLines.IsNullOrEmpty())
                    {
                        fundingValue.FundingLines.ToList().ForEach(x => ValidateFundingLine(context, x));
                    }
                });
        }

        private void ValidateFundingLine(CustomContext context, FundingLine fundingLine)
        {
            if(!fundingLine.DistributionPeriods.IsNullOrEmpty())
            {
                context.AddFailure("DistributionPeriods", $"Funding line : '{fundingLine.Name}' has values for the distribution periods");
            }

            fundingLine.FundingLines?.ToList().ForEach(x => ValidateFundingLine(context, x));

            fundingLine.Calculations?.ToList().ForEach(x => ValidateCalculation(context, x));
        }

        private void ValidateCalculation(CustomContext context, Calculation calculation)
        {
            string calculationName = calculation.Name.Trim().ToLower();
            ICollection<uint> existingTemplateCalculationIds = _calculationTemplateCalcIds.GetOrAdd(calculationName, _ => new HashSet<uint>());
            existingTemplateCalculationIds.Add(calculation.TemplateCalculationId);

            if (existingTemplateCalculationIds.Any(_ => _ != calculation.TemplateCalculationId))
            {
                context.AddFailure("Calculation", $"Calculation name: '{calculationName}' is present multiple times in the template but with a different templateCalculationIds.");   
            }

            Calculation existingCalculation = _calculationDictionary.GetOrAdd(calculation.TemplateCalculationId, calculation);
            
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
                            calculation.ReferenceData?.ToList().ForEach(x => ValidateReferenceData(context, x, existingCalculation));
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
                ValidateCalculation(context, nestedCalculation);
            }
        }

        private void ValidateReferenceData(CustomContext context, ReferenceData referenceData, Calculation existingCalculation)
        {
            ReferenceData currentReferenceData = existingCalculation.ReferenceData.Where(x => x.TemplateReferenceId == referenceData.TemplateReferenceId).SingleOrDefault();

            if (currentReferenceData != null)
            {
                ReferenceData existingReferenceData = _referenceDataDictionary.GetOrAdd(currentReferenceData.TemplateReferenceId, currentReferenceData);

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
