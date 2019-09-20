using System;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class SetFundingStreamPeriodProfilePatternRequestModel
    {
        public string FundingPeriodId { get; set; }

        public string FundingStreamId { get; set; }

        public string FundingLineId { get; set; }

        public DateTime? FundingStreamPeriodStartDate { get; set; }

        public DateTime? FundingStreamPeriodEndDate { get; set; }

        public bool ReProfilePastPeriods { get; set; }

        public bool CalculateBalancingPayment { get; set; }

        public ProfilePeriodPattern[] ProfilePattern { get; set; }
    }
}
