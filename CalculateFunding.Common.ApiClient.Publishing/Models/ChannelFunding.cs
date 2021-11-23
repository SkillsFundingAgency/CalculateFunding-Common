namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ChannelFunding
    {
        public string ChannelCode { get; set; }
        public string ChannelName { get; set; }
        public decimal? TotalFunding { get; set; }
        public int TotalProviders { get; set; }
    }
}