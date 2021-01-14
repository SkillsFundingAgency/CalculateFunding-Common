using CalculateFunding.Common.Models;
using System;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class FundingLineProfile
    {
        public string FundingLineCode { get; set; }
        public string FundingLineName { get; set; }
        public decimal? TotalAllocation { get; set; }
        public decimal AmountAlreadyPaid { get; set; }
        public decimal? RemainingAmount { get; set; }
        public decimal? CarryOverAmount { get; set; }
        public string ProviderId { get; set; }
        public string ProviderName { get; set; }
        public string UKPRN { get; set; }
        public string ProfilePatternKey { get; set; }
        public string ProfilePatternName { get; set; }
        public string ProfilePatternDescription { get; set; }
        public Reference LastUpdatedUser { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public decimal? ProfileTotalAmount { get; set; }
        public IEnumerable<ProfileTotal> ProfileTotals { get; set; }
    }
}
