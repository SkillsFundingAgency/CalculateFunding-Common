using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Providers.Models
{
    public class ProviderVersion : ProviderVersionMetadata
    {
        public IEnumerable<Provider> Providers { get; set; }
    }
}
