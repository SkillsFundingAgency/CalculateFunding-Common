using System;
using CalculateFunding.Generators.Funding.Enums;
using CalculateFunding.Generators.Funding.Models;
using CalculateFunding.Common.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Generators.Funding
{
    public class FundingGenerator
    {
        public FundingValue GenerateFundingValue(IEnumerable<FundingLine> fundingLines, int fundingLineDecimalPlaces = 2)
        {
            List<FundingLine> fundingLinesList = fundingLines.ToList();

            return new FundingValue
            {
                TotalValue = fundingLinesList.NullableSum(fundingLine =>
                {
                    SetFundingLinesTotalValues(fundingLine, fundingLineDecimalPlaces);
                    return CalculateFundingTotal(fundingLine);
                }),
                FundingLines = fundingLinesList
            };
        }

        private decimal? CalculateFundingTotal(FundingLine fundingLine)
        {
            decimal? paymentFundingLineValue = fundingLine.Type == FundingLineType.Payment ? fundingLine.Value : null;
            decimal? subFundingLinesTotal = fundingLine.FundingLines?.NullableSum(CalculateFundingTotal);
            decimal? nonPaymentFundingLineSubTotal = fundingLine.Type != FundingLineType.Payment ? (subFundingLinesTotal) : null;

            return paymentFundingLineValue.AddValueIfNotNull(nonPaymentFundingLineSubTotal);
        }

        private static decimal? SetFundingLinesTotalValues(FundingLine fundingLine, int decimalPlaces)
        {
            decimal? cashCalculationsSum = GetCashCalculationsSum(fundingLine);

            decimal? subFundingLinesTotalValue = null;
            if (fundingLine.FundingLines != null)
            {
                subFundingLinesTotalValue = fundingLine.FundingLines.Select(_ => SetFundingLinesTotalValues(_, decimalPlaces))
                    .Where(fundingLineTotal => fundingLineTotal != null)
                    .Aggregate(
                        subFundingLinesTotalValue, 
                        (current, fundingLineTotal) => current.AddValueIfNotNull(fundingLineTotal));
            }

            decimal? fundingLineValue = cashCalculationsSum.AddValueIfNotNull(subFundingLinesTotalValue);

            fundingLine.Value = fundingLineValue.HasValue ? Math.Round(fundingLineValue.Value, decimalPlaces, MidpointRounding.AwayFromZero) : fundingLineValue;

            return fundingLine.Value;
        }

        private static decimal? GetCashCalculationsSum(FundingLine fundingLine)
        {
            List<Calculation> cashCalculations = fundingLine.Calculations?
                .Where(calculation => calculation.Type == CalculationType.Cash)
                .ToList();

            decimal? cashCalculationsSum = null;

            if (cashCalculations != null && cashCalculations.Any(c => c.GetValueAsNullableDecimal() != null))
                cashCalculationsSum = cashCalculations.NullableSum(GetCalculationsTotalRecursive);

            return cashCalculationsSum;
        }

        private static decimal? GetCalculationsTotalRecursive(Calculation calculation)
        {
            if(calculation.Type == CalculationType.Cash)
            {
                return calculation.GetValueAsNullableDecimal();
            }
            else
            {
                decimal? calculationSum = calculation.Calculations?
                    .Where(subCalculation => subCalculation.Type == CalculationType.Cash)
                    .NullableSum(GetCalculationsTotalRecursive);

                return calculation.GetValueAsNullableDecimal().AddValueIfNotNull(calculationSum);
            }
        }
    }
}
