using Polly;

namespace CalculateFunding.Generators.OrganisationGroup.Interfaces
{
    public class OrganisationGroupResiliencePolicies : IOrganisationGroupResiliencePolicies
    {
        public AsyncPolicy ProvidersApiClient { get; set; }
    }
}
