using System;
using CalculateFunding.Common.ApiClient.Models;
using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class CalculationCurrentVersion : Reference
    {
        public string SpecificationId { get; set; }

        public string FundingPeriodName { get; set; }

        public string FundingStreamId { get; set; }

        public string SourceCode { get; set; }

        public DateTimeOffset? Date { get; set; }

        public Reference Author { get; set; }

        public int Version { get; set; }

        public string CalculationType { get; set; }

        public PublishStatus PublishStatus { get; set; }

        public string SourceCodeName { get; set; }
    }
}
