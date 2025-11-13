using System.Linq;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Common.TemplateMetadata.Enums;
using CalculateFunding.Common.TemplateMetadata.Models;
using CalculateFunding.Common.TemplateMetadata.Schema12.Models;
using AggregationType = CalculateFunding.Common.TemplateMetadata.Enums.AggregationType;
using FundingLineType = CalculateFunding.Common.TemplateMetadata.Enums.FundingLineType;

namespace CalculateFunding.Common.TemplateMetadata.Schema12.Mapping
{
    public static class TemplateModelHelper
    {
        private static Calculation ToCalculation(this SchemaJsonCalculation source)
        {
            return new Calculation
            {
                Name = source.Name,
                ValueFormat = source.ValueFormat.AsMatchingEnum<CalculationValueFormat>(),
                AggregationType = source.AggregationType.AsMatchingEnum<AggregationType>(),
                Type = source.Type.AsMatchingEnum<CalculationType>(),
                TemplateCalculationId = source.TemplateCalculationId,
                FormulaText = source.FormulaText,
                Calculations = source.Calculations?.Select(ToCalculation),
                GroupRate = ToGroupRate(source.GroupRate),
                PercentageChangeBetweenAandB = ToPercentageChangeBetweenAandB(source.PercentageChangeBetweenAandB),
                AllowedEnumTypeValues = source.AllowedEnumTypeValues?.Any() == true ? source.AllowedEnumTypeValues : null
            };
        }

        private static GroupRate ToGroupRate(SchemaJsonGroupRate source)
        {
            return source == null
                ? null
                : new GroupRate
                {
                    Denominator = source.Denominator,
                    Numerator = source.Numerator
                };
        }

        private static PercentageChangeBetweenAandB ToPercentageChangeBetweenAandB(SchemaJsonPercentageChangeBetweenAandB source)
        {
            return source == null
                ? null
                : new PercentageChangeBetweenAandB
                {
                    CalculationA = source.CalculationA,
                    CalculationB = source.CalculationB,
                    CalculationAggregationType = source.CalculationAggregationType.AsMatchingEnum<AggregationType>()
                };
        }

        public static FundingLine ToFundingLine(this SchemaJsonFundingLine source)
        {
            return new FundingLine
            {
                Name = source.Name,
                ExternalFundingLineName = source.ExternalFundingLineName,
                TemplateLineId = source.TemplateLineId,
                FundingLineCode = source.FundingLineCode,
                Type = source.Type.AsMatchingEnum<FundingLineType>(),
                Calculations = source.Calculations?.Select(ToCalculation),
                FundingLines = source.FundingLines?.Select(ToFundingLine)
            };
        }
    }
}