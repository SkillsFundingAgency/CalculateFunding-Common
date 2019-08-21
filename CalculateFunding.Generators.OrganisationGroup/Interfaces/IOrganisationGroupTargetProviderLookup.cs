using System.Collections.Generic;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Generators.OrganisationGroup.Models;

namespace CalculateFunding.Generators.OrganisationGroup.Interfaces
{
    public interface IOrganisationGroupTargetProviderLookup
    {
        Task<TargetOrganisationGroup> GetTargetProviderDetails(string organisationIdentifier, GroupingReason groupingReason, OrganisationGroupTypeCode organisationGroupTypeCode, OrganisationGroupTypeIdentifier groupTypeIdentifier, string providerVersionId, IEnumerable<Common.ApiClient.Providers.Models.Provider> providers);
    }
}