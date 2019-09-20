using System;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class ProfilePeriodPattern
    {
        public PeriodType PeriodType { get; set; }

        public string Period { get; set; }

        public DateTime PeriodStartDate { get; set; }

        public DateTime PeriodEndDate { get; set; }

        public int PeriodYear { get; set; }

        public int Occurrence { get; set; }

        public string DistributionPeriod { get; set; }

        public decimal PeriodPatternPercentage { get; set; }
    }
}
