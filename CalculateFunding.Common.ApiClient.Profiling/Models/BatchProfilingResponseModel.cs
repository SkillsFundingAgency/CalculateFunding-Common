namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public class BatchProfilingResponseModel : ProviderProfilingResponseModel
    {
        public string Key { get; set; }
        
        public decimal FundingValue { get; set; }
    }
}