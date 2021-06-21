using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class PublishedSpecificationConfiguration
    {
        public IEnumerable<PublishedSpecificationItem> FundingLines { get; set; }

        public IEnumerable<PublishedSpecificationItem> Calculations { get; set; }

        public string FundingStreamId { get; set; }

        public string FundingPeriodId { get; set; }

        public string SpecificationId { get; set; }
    }
}