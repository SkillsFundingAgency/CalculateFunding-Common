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
            var fundingLinesList = fundingLines.ToList();

            return new FundingValue
            {
                TotalValue = fundingLinesList.Sum(fundingLine =>
                {
                    GetFundingLinesTotalValueRecursive(fundingLine);
                    return CalculateFundingTotal(fundingLine);
                }),
                FundingLines = fundingLinesList
            };
        }

        private decimal? CalculateFundingTotal(FundingLine fundingLine)
        {
            decimal? paymentFundingLineValue = fundingLine.Type == FundingLineType.Payment ? fundingLine.Value : 0;
            decimal? subFundingLinesTotal = fundingLine.FundingLines?.Sum(CalculateFundingTotal);
            decimal? nonPaymentFundingLineSubTotal = fundingLine.Type != FundingLineType.Payment ? (subFundingLinesTotal) : 0;

            return paymentFundingLineValue + nonPaymentFundingLineSubTotal;
        }

        private static decimal? GetFundingLinesTotalValueRecursive(FundingLine fundingLine)
        {
            decimal? cashCalculationsSum = GetCashCalculationsSum(fundingLine);

            decimal? subFundingLinesTotalValue = null;
            if (fundingLine.FundingLines != null)
            {
                subFundingLinesTotalValue = fundingLine.FundingLines.Select(GetFundingLinesTotalValueRecursive)
                    .Where(fundingLineTotal => fundingLineTotal != null)
                    .Aggregate(
                        subFundingLinesTotalValue, 
                        (current, fundingLineTotal) => current.AddValueIfNotNull(fundingLineTotal));
            }

            if (cashCalculationsSum != null && subFundingLinesTotalValue == null)
                fundingLine.Value = cashCalculationsSum;
            else if (cashCalculationsSum == null && subFundingLinesTotalValue != null)
                fundingLine.Value = subFundingLinesTotalValue;
            else if (cashCalculationsSum != null)
                fundingLine.Value = cashCalculationsSum + subFundingLinesTotalValue;

            return fundingLine.Value;
        }

        private static decimal? GetCashCalculationsSum(FundingLine fundingLine)
        {
            List<Calculation> cashCalculations = fundingLine.Calculations?
                .Where(calculation => calculation.Type == CalculationType.Cash).ToList();

            decimal? cashCalculationsSum = null;

            if (cashCalculations != null && cashCalculations.Any(c => c.Value != null))
                cashCalculationsSum = cashCalculations.Sum(GetCalculationsTotalRecursive);

            return cashCalculationsSum;
        }

        private static decimal? GetCalculationsTotalRecursive(Calculation calculation)
        {
            if(calculation.Type == CalculationType.Cash)
            {
                return calculation.Value;
            }
            else
            {
                IEnumerable<Calculation> cashCalculations = calculation.Calculations?
                .Where(subCalculation => subCalculation.Type == CalculationType.Cash);

                decimal? calculationSum = cashCalculations.Sum(GetCalculationsTotalRecursive);

                return calculation.Value.AddValueIfNotNull(calculationSum);
            }
        }
    }
}
