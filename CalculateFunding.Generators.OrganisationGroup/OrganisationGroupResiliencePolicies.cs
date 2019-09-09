using Polly;

namespace CalculateFunding.Generators.OrganisationGroup.Interfaces
{
    public class OrganisationGroupResiliencePolicies : IOrganisationGroupResiliencePolicies
    {
        public Policy ProvidersApiClient { get; set; }
    }
}
