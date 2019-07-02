using CalculateFunding.Common.Models;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    public class FundingStream : Reference
    {
        public FundingStream()
        {
            AllocationLines = new List<AllocationLine>();
            PeriodType = new PeriodType();
        }

        public string ShortName { get; set; }

        public List<AllocationLine> AllocationLines { get; set; }

        public PeriodType PeriodType { get; set; }

        public bool RequireFinancialEnvelopes { get; set; }
    }
}
