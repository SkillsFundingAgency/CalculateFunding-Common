using System;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ReleaseProvidersToChannelRequest
    {
        public IEnumerable<string> Channels { get; set; }

        public IEnumerable<string> ProviderIds { get; set; }
    }
}
