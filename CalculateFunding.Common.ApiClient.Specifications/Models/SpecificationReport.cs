using System;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    public class SpecificationReport
    {
        public string SpecificationReportIdentifier { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public string Format { get; set; }
        public string Size { get; set; }
    }
}
