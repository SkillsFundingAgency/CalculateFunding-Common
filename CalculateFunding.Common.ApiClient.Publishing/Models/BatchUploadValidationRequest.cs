namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class BatchUploadValidationRequest
    {
        public string FundingStreamId { get; set; }
        
        public string FundingPeriodId { get; set; }
        
        public string BatchId { get; set; }
    }
}