using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Common.ApiClient.Providers.Models;
using CalculateFunding.Generators.OrganisationGroup.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalculateFunding.Generators.OrganisationGroup
{
    public class ProviderFilter : IProviderFilter
    {
        public bool ShouldIncludeProvider(Provider provider, IEnumerable<ProviderTypeMatch> providerTypeMatches)
            => providerTypeMatches.Any(providerTypeMatch => string.Equals(provider.ProviderType, providerTypeMatch.ProviderType, StringComparison.InvariantCultureIgnoreCase) &&
                    string.Equals(provider.ProviderSubType, providerTypeMatch.ProviderSubtype, StringComparison.InvariantCultureIgnoreCase));

        public bool ShouldIncludeProvider(Provider provider, IEnumerable<string> providerStatus) =>
            providerStatus.Any(status => string.Equals(provider.Status, status, StringComparison.InvariantCultureIgnoreCase));
    }
}
