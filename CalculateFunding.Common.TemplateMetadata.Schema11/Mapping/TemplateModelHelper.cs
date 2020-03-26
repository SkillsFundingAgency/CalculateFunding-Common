using System;
using System.Linq;
using CalculateFunding.Common.TemplateMetadata.Enums;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.TemplateMetadata.Schema11.Models;

namespace CalculateFunding.Common.TemplateMetadata.Schema11.Mapping
{
    public static class TemplateModelHelper
    {
        public static Calculation ToCalculation(this SchemaJsonCalculation source)
        {
            return new Calculation
            {
                Name = source.Name,
                ValueFormat = (CalculationValueFormat) Enum.Parse(typeof(CalculationValueFormat), source.ValueFormat),
                AggregationType = (AggregationType) Enum.Parse(typeof(AggregationType), source.AggregationType),
                Type = (CalculationType) Enum.Parse(typeof(CalculationType), source.Type),
                TemplateCalculationId = source.TemplateCalculationId,
                FormulaText = source.FormulaText,
                Calculations = source.Calculations?.Select(x => ToCalculation(x))
            };
        }
        
        public static FundingLine ToFundingLine(this SchemaJsonFundingLine source)
        {
            return new FundingLine
            {
                Name = source.Name,
                TemplateLineId = source.TemplateLineId,
                FundingLineCode = source.FundingLineCode,
                Type = (FundingLineType) Enum.Parse(typeof(FundingLineType), source.Type),
                Calculations = source.Calculations?.Select(calculationMap => ToCalculation(calculationMap)),
                FundingLines = source.FundingLines?.Select(x => ToFundingLine(x))
            };
        }
    }
}