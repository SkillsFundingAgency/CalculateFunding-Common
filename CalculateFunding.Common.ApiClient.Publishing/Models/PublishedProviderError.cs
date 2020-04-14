namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class PublishedProviderError
    {
        public string FundingLineCode { get; set; }
        
        public PublishedProviderErrorType Type { get; set; }
        
        public string Description { get; set; }
    }
}