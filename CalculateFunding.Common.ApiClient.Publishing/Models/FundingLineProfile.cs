using CalculateFunding.Common.Models;
using System;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class FundingLineProfile
    {
        public decimal? TotalAllocation { get; set; }
        public decimal AmountAlreadyPaid { get; set; }
        public decimal? RemainingAmount { get; set; }
        public decimal? CarryOverAmount { get; set; }
        public string ProviderName { get; set; }
        public string ProfilePatternKey { get; set; }
        public Reference LastUpdatedUser { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public IEnumerable<ProfileTotal> ProfileTotals { get; set; }
    }
}
