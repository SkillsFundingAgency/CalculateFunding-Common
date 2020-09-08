using CalculateFunding.Common.Models;
using System;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class FundingLineChange
    {
        public decimal? FundingLineTotal { get; set; }
        public decimal? PreviousFundingLineTotal { get; set; }
        public string FundingStreamName { get; set; }
        public string FundingLineName { get; set; }
        public decimal? CarryOverAmount { get; set; }
        public Reference LastUpdatedUser { get; set; }
        public DateTimeOffset? LastUpdatedDate { get; set; }
        public IEnumerable<ProfileTotal> ProfileTotals { get; set; }
    }
}
