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
                ValueFormat = (CalculationValueFormat) Enum.Parse(typeof(CalculationValueFormat), source.ValueFormat.ToString()),
                AggregationType = (Enums.AggregationType) Enum.Parse(typeof(Enums.AggregationType), source.AggregationType.ToString()),
                Type = (CalculationType) Enum.Parse(typeof(CalculationType), source.Type.ToString()),
                TemplateCalculationId = source.TemplateCalculationId,
                FormulaText = source.FormulaText,
                Calculations = source.Calculations?.Select(ToCalculation)
            };
        }
        
        public static FundingLine ToFundingLine(this SchemaJsonFundingLine source)
        {
            return new FundingLine
            {
                Name = source.Name,
                TemplateLineId = source.TemplateLineId,
                FundingLineCode = source.FundingLineCode,
                Type = (Enums.FundingLineType) Enum.Parse(typeof(Enums.FundingLineType), source.Type.ToString()),
                Calculations = source.Calculations?.Select(ToCalculation),
                FundingLines = source.FundingLines?.Select(ToFundingLine)
            };
        }
    }
}