using CalculateFunding.Generators.Funding.Enums;
using CalculateFunding.Generators.Funding.Models;
using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Generators.Funding
{
    public class FundingGenerator
    {
        public FundingValue GenerateFundingValue(IEnumerable<FundingLine> fundingLines)
        {
            FundingValue fundingValue = new FundingValue { TotalValue = fundingLines.Sum(_ =>
            {
                ToFundingLineTotal(_);
                return ToFundingTotal(_);
            }),
            FundingLines = fundingLines };

            return fundingValue;
        }

        private decimal ToFundingTotal(FundingLine fundingLine)
        {
            return (fundingLine.Type == FundingLineType.Payment ? fundingLine.Value : 0) + (fundingLine.Type != FundingLineType.Payment ? (fundingLine.FundingLines?.Sum(_ => ToFundingTotal(_)) ?? 0) : 0);
        }

        private decimal ToFundingLineTotal(FundingLine fundingLine)
        {
            return fundingLine.Value = (fundingLine.Calculations?.Where(_ => _.Type == Enums.CalculationType.Cash)?.Sum(_ => ToCalculationTotal(_)) ?? 0) + (fundingLine.FundingLines?.Sum(_ => ToFundingLineTotal(_)) ?? 0);
        }

        private decimal ToCalculationTotal(Calculation calculation)
        {
            return calculation.Value + (calculation.Calculations?.Where(_ => _.Type == Enums.CalculationType.Cash)?.Sum(_ => ToCalculationTotal(_)) ?? 0);
        }
    }
}
