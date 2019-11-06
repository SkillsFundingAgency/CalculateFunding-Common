using System;
using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    public class FundingPeriod : Reference
    {
        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public string Period { get; set; }

        public FundingPeriodType? Type { get; set; }

        public int StartYear
        {
            get
            {
                return StartDate.Year;
            }
        }

        public int EndYear
        {
            get
            {
                return EndDate.Year;
            }
        }
    }
}
