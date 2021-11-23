using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Publishing
{
    public class ReleaseFundingPublishProvidersRequest
    {
        public IEnumerable<string> PublishedProviderIds { get; set; }
        public IEnumerable<string> ChannelCodes { get; set; }
    }
}