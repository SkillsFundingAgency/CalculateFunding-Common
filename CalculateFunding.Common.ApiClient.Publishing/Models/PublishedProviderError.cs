namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class PublishedProviderError
    {
        public string FundingLineCode { get; set; }
        
        public PublishedProviderErrorType Type { get; set; }

        public string SummaryErrorMessage { get; set; }

        public string DetailedErrorMessage { get; set; }

        public FundingLine FundingLine { get; set; }

        public string FundingStreamId { get; set; }
    }
}