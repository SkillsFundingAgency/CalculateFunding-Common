using System;
using System.Collections.Generic;
using System.Linq;
using CalculateFunding.Common.Extensions;
using CalculateFunding.Generators.Funding.Enums;
using CalculateFunding.Generators.Funding.Models;

namespace CalculateFunding.Generators.Funding
{
    public class FundingGenerator
    {
        public FundingValue GenerateFundingValue(IEnumerable<FundingLine> fundingLines,
            int fundingLineDecimalPlaces = 2)
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
            decimal? nonPaymentFundingLineSubTotal = fundingLine.Type != FundingLineType.Payment ? subFundingLinesTotal : null;

            return paymentFundingLineValue.AddValueIfNotNull(nonPaymentFundingLineSubTotal);
        }

        private static decimal? SetFundingLinesTotalValues(FundingLine fundingLine,
            int decimalPlaces)
        {
            decimal? cashCalculationsSum = GetCashCalculationsSum(fundingLine);

            decimal? subFundingLinesTotalValue = null;
            if (fundingLine.FundingLines != null)
            {
                subFundingLinesTotalValue = fundingLine.FundingLines.Select(_ => SetFundingLinesTotalValues(_, decimalPlaces))
                    .Where(fundingLineTotal => fundingLineTotal != null)
                    .Aggregate(
                        (decimal?)null,
                        (current,
                            fundingLineTotal) => current.AddValueIfNotNull(fundingLineTotal));
            }

            decimal? fundingLineValue = cashCalculationsSum.AddValueIfNotNull(subFundingLinesTotalValue);

            fundingLine.Value = fundingLineValue.HasValue ? Math.Round(fundingLineValue.Value, decimalPlaces, MidpointRounding.AwayFromZero) : (decimal?)null;

            return fundingLine.Value;
        }

        private static decimal? GetCashCalculationsSum(FundingLine fundingLine)
        {
            List<Calculation> cashCalculations = fundingLine.Calculations?
                .Where(IsCountedAsCash)
                .ToList();

            decimal? cashCalculationsSum = null;

            if (cashCalculations != null && cashCalculations.Any(c => c.GetValueAsNullableDecimal() != null))
                cashCalculationsSum = cashCalculations.NullableSum(GetCalculationsTotalRecursive);

            return cashCalculationsSum;
        }

        private static decimal? GetCalculationsTotalRecursive(Calculation calculation)
        {
            if (IsCountedAsCash(calculation))
            {
                return calculation.GetValueAsNullableDecimal();
            }

            decimal? calculationSum = calculation.Calculations?
                .Where(IsCountedAsCash)
                .NullableSum(GetCalculationsTotalRecursive);

            return calculation.GetValueAsNullableDecimal().AddValueIfNotNull(calculationSum);
        }

        private static bool IsCountedAsCash(Calculation calculation)
            => calculation.Type == CalculationType.Cash ||
               calculation.Type == CalculationType.Adjustment;
    }
}