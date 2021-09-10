using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Providers.Models;
using System.Collections.Generic;

namespace CalculateFunding.Generators.OrganisationGroup.Interfaces
{
    public interface IProviderFilter
    {
        bool ShouldIncludeProvider(Provider provider, IEnumerable<ProviderTypeMatch> providerTypeMatches);

        bool ShouldIncludeProvider(Provider provider, IEnumerable<string> providerStatus);
    }
}
