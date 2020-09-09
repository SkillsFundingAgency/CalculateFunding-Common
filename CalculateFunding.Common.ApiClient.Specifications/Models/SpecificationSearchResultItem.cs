using System;
using System.Collections.Generic;
using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    public class SpecificationSearchResultItem : Reference
    {
        public string FundingPeriodName { get; set; }

        public string FundingPeriodId { get; set; }

        public IEnumerable<string> FundingStreamNames { get; set; }

        public IEnumerable<string> FundingStreamIds { get; set; }

        public DateTimeOffset LastUpdatedDate { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

        public bool IsSelectedForFunding { get; set; }
    }
}
