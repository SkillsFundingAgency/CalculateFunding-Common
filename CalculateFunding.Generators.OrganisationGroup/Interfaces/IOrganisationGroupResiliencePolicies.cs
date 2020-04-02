using Polly;

namespace CalculateFunding.Generators.OrganisationGroup.Interfaces
{
    public interface IOrganisationGroupResiliencePolicies
    {
        AsyncPolicy ProvidersApiClient { get; set; }
    }
}
