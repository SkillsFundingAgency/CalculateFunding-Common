using System.Collections.Generic;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Policies.Models;
using CalculateFunding.Generators.OrganisationGroup.Models;

namespace CalculateFunding.Generators.OrganisationGroup.Interfaces
{
    public interface IOrganisationGroupTargetProviderLookup
    {
        /// <summary>
        /// Returns the target organisations groups details, based on the configuration of the funding output.
        /// eg will lookup
        /// - Parlimentary Consituency code and name, based on the Parlimentary Consituency code for information
        /// </summary>
        /// <param name="organisationGroupLookupParameters">Grouping Lookup Parameters</param>
        /// <param name="groupingReason">Grouping Reason</param>
        /// <param name="providersInGroup">Providers in group</param>
        /// <returns></returns>
        Task<TargetOrganisationGroup> GetTargetProviderDetails(OrganisationGroupLookupParameters organisationGroupLookupParameters, GroupingReason groupReason, IEnumerable<Common.ApiClient.Providers.Models.Provider> providersInGroup);
    }
}