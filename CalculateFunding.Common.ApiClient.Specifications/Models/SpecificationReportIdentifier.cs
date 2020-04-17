namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    public class SpecificationReportIdentifier
    {
        public JobType JobType { get; set; }
        public string SpecificationId { get; set; }
        public string FundingStreamId { get; set; }
        public string FundingPeriodId { get; set; }
        public string FundingLineCode { get; set; }
    }
}
