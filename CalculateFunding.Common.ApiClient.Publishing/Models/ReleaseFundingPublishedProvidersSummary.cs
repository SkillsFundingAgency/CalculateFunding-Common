using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ReleaseFundingPublishedProvidersSummary
    {
        public ReleaseFundingPublishedProvidersSummary()
        {
            ChannelFundings = new List<ChannelFunding>();
        }

        public int TotalProviders { get; set; }
        public int TotalIndicativeProviders { get; set; }
        public decimal? TotalFunding { get; set; }
        public IEnumerable<ChannelFunding> ChannelFundings { get; set; }
    }
}
