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
    internal class TemplateMetadataValidator : AbstractValidator<FeedBaseModel>
    {
        private readonly ConcurrentDictionary<uint, FundingLine> fundingDictionary;
        private readonly ConcurrentDictionary<uint, Calculation> calculationDictionary;
        private readonly ConcurrentDictionary<uint, ReferenceData> referenceDataDictionary;

        internal TemplateMetadataValidator()
        {
            fundingDictionary = new ConcurrentDictionary<uint, FundingLine>();
            calculationDictionary = new ConcurrentDictionary<uint, Calculation>();
            referenceDataDictionary = new ConcurrentDictionary<uint, ReferenceData>();

            RuleFor(model => model.Funding.FundingValue.FundingValueByDistributionPeriod.FirstOrDefault())
                .NotEmpty()
                .WithMessage("No funding valu distribution period provided for TemplateMetadataValidator")
                .Custom((fundingValuDistributionPeriod, context) =>
                {
                    if (!fundingValuDistributionPeriod.FundingLines.IsNullOrEmpty())
                    {
                        ValidateFundingLines(context, fundingValuDistributionPeriod.FundingLines);
                    }
                });
        }

        private void ValidateFundingLines(CustomContext context, IEnumerable<FundingLine> fundingLines)
        {
            if(fundingLines.IsNullOrEmpty())
            {
                return;
            }

            foreach(FundingLine fundingLine in fundingLines)
            {
                FundingLine existingFundingLine = fundingDictionary.GetOrAdd(fundingLine.TemplateLineId, fundingLine);

                if(!existingFundingLine.ProfilePeriods.IsNullOrEmpty())
                {
                    context.AddFailure("ProfilePeriods", $"Funding line : '{existingFundingLine.Name}' has values for the profilePeriods");
                }

                ValidateFundingLines(context, fundingLine.FundingLines);

                if (existingFundingLine.Name == fundingLine.Name)
                {
                    if (fundingLine.Calculations.IsNullOrEmpty())
                    {
                        continue;
                    }

                    foreach(Calculation calculation in fundingLine.Calculations)
                    {
                        Calculation existingCalculation = calculationDictionary.GetOrAdd(calculation.TemplateCalculationId, calculation);

                        if(existingCalculation.Name != calculation.Name || existingCalculation.AggregationType != calculation.AggregationType || existingCalculation.Type != calculation.Type || existingCalculation.ValueFormat != calculation.ValueFormat || existingCalculation.FormulaText != calculation.FormulaText)
                        {
                            context.AddFailure("Calculation", $"Calculation : '{existingCalculation.Name}' and id : '{calculation.TemplateCalculationId}' has calculation items with the same templateCalculationId that have different configurations for 'name', 'valueFormat','type'.'formulaText','aggregationType'.");
                        }
                        else
                        {
                            if(!existingCalculation.ReferenceData.IsNullOrEmpty())
                            {
                                if(!calculation.ReferenceData.IsNullOrEmpty())
                                {
                                    if(existingCalculation.ReferenceData.Count() != calculation.ReferenceData.Count() || !existingCalculation.ReferenceData.Any(x => calculation.ReferenceData.Any(y => y.TemplateReferenceId == x.TemplateReferenceId)))
                                    {
                                        context.AddFailure("ReferenceData", $"Calculation : '{existingCalculation.Name}' and id : '{existingCalculation.TemplateCalculationId}' reference data doesn't match all templateReferenceId's for calculation : '{existingCalculation.Name}' and id : '{existingCalculation.TemplateCalculationId}'.");
                                    }
                                    else
                                    {
                                        foreach (ReferenceData referenceData in calculation.ReferenceData)
                                        {
                                            ReferenceData currentReferenceData = existingCalculation.ReferenceData.Where(x => x.TemplateReferenceId == referenceData.TemplateReferenceId).SingleOrDefault();

                                            if (currentReferenceData != null)
                                            {
                                                ReferenceData existingReferenceData = referenceDataDictionary.GetOrAdd(currentReferenceData.TemplateReferenceId, currentReferenceData);

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
                                else
                                {
                                    context.AddFailure("ReferenceData", $"Reference : '{existingCalculation.ReferenceData.First().Name}' doesn't exist on Calculation : {calculation.Name}.");
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
                    }
                }
                else
                {
                    context.AddFailure("FundingLine", $"Funding line : '{fundingLine.Name}' is not unique.");
                }
            }
        }
    }
}
