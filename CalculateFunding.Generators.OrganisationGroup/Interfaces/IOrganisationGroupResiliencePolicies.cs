using Polly;

namespace CalculateFunding.Generators.OrganisationGroup.Interfaces
{
    public interface IOrganisationGroupResiliencePolicies
    {
        Policy ProvidersApiClient { get; set; }
    }
}
