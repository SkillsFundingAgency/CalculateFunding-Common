using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ApplyCustomProfileRequest
    {
        public string FundingStreamId { get; set; }

        public string FundingPeriodId { get; set; }

        public string FundingLineCode { get; set; }

        public string ProviderId { get; set; }
        
        public string CustomProfileName { get; set; }

        public string PublishedProviderId => $"publishedprovider-{ProviderId}-{FundingPeriodId}-{FundingStreamId}";

        public IEnumerable<ProfilePeriod> ProfilePeriods { get; set; }

        public decimal? CarryOver { get; set; }
    }
}
