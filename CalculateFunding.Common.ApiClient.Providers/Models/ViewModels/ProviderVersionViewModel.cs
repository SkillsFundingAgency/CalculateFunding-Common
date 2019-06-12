using CalculateFunding.Common.ApiClient.Providers.Models;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Providers.ViewModels
{
    public class ProviderVersionViewModel : ProviderVersionMetadata
    {
        public IEnumerable<Provider> Providers { get; set; }
    }
}
